using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Azure;
using System.Threading;
using TheOmenDen.CrowsOnTheShelf.Server.Hubs;
using TheOmenDen.CrowsOnTheShelf.Server.Spaces;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsOnTheShelf.Server.Lobbies;

public class Lobby
{
    private readonly ILogger<Lobby> _logger;
    private readonly IHubContext<SecretSantaHub> _hubContext;
    private readonly List<Room> _rooms = new (10);
    private readonly Dictionary<Participant, Room> _participantToRoomsDictionary = new(30);
    private readonly string _lobbyGroupName = Guid.NewGuid().ToString();

    public Lobby(IHubContext<SecretSantaHub> hubContext, ILogger<Lobby> logger)
    {
        _logger= logger;
        _hubContext= hubContext;

        for (var i = 0; i < 15; i++)
        {
            AddRoom(new PublicRoom(_hubContext, $"Room {i}", new RoomSettings(), EventEndedAsync));
        }
    }

    internal void AddRoom(Room room)
    {
        int count; 
        lock (_rooms)
        {
            _rooms.Add(room);
            count= _rooms.Count;
        }

        _logger.LogInformation("Room added. Index: {Index}. Name: {Name}. Active Rooms: {Count}", room.RoomIndex,
            room.RoomName, count);
    }

    internal IEnumerable<Participant>? GetParticipantsInRoom(String roomName)
    {
        Room? room;
        lock (_rooms)
        {
            room = _rooms.FirstOrDefault(r => r.RoomName.Equals(roomName));
        }

        return room?.Participants ?? Enumerable.Empty<Participant>();
    }

    internal Task AddParticipantAsync(Participant participant, CancellationToken cancellationToken = default)
        => _hubContext.Groups.AddToGroupAsync(participant.ConnectionId, _lobbyGroupName, cancellationToken: cancellationToken);

    internal Task RemoveParticipant(Participant participant, CancellationToken cancellationToken = default)
    => _hubContext.Groups.RemoveFromGroupAsync(participant.ConnectionId, _lobbyGroupName, cancellationToken: cancellationToken);

    internal async Task ParticipantReconnected(Participant participant, Room room, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.GroupExcept(room.RoomName, participant.ConnectionId)
            .SendAsync("ParticipantConnectionStatusChanged", participant.ToDTO(), cancellationToken: cancellationToken);

        await _hubContext.Groups.AddToGroupAsync(participant.ConnectionId, room.RoomName, cancellationToken);

        await room.AddParticipantAsync(participant, true, cancellationToken);
    }

    internal async Task ParticipantDisconnected(Participant participant, Room room,
        CancellationToken cancellationToken = default)
    {
        participant.IsConnected = false;
        if (room.RoomState is RoomStateLobby)
        {
            await LeaveRoomAsync(participant, room, cancellationToken);
            return;
        }

        await _hubContext.Clients.GroupExcept(room.RoomName, participant.ConnectionId)
            .SendAsync("ParticipantConnectionStatusChanged", participant.ToDTO(), cancellationToken: cancellationToken);
    }

    internal IEnumerable<RoomStateDTO> GetRooms()
    {
        lock (_rooms)
        {
            return _rooms.Select(r => r.ToRoomStateDto());
        }
    }

    internal async Task<RoomStateDTO?> JoinRoomAsync(Participant participant, Room newRoom, String code, CancellationToken cancellationToken = default)
    {
        if (newRoom is null || participant is null)
        {
            return null;
        }

        if (newRoom.RoomSettings.IsPrivateRoom
            && !newRoom.RoomSettings.GameCode.Equals(code))
        {
            return null;
        }

        await newRoom.AddParticipantAsync(participant, cancellationToken: cancellationToken);

        var newRoomDto = newRoom.ToRoomStateDto();

        await _hubContext.Groups.AddToGroupAsync(participant.ConnectionId, newRoom.RoomName, cancellationToken);
        
        await _hubContext.Clients
            .GroupExcept(newRoom.RoomName, participant.ConnectionId)
            .SendAsync("ParticipantJoined", participant.ToDTO(), cancellationToken: cancellationToken);
        
        await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomStateChanged", newRoomDto, cancellationToken: cancellationToken);

        return newRoomDto;
    }

    internal async Task<bool> StartEventAsync(Room room, Participant participant,
        CancellationToken cancellationToken = default)
    {
        if (!room.StartEvent(participant))
        {
            return false;
        }

        await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomStateChanged", room.ToRoomStateDto(), cancellationToken: cancellationToken);
            
        _logger.LogInformation("New event started in room {Room} with {Players} players", room.RoomName,
            room.Participants.Count);

        return true;

    }

    internal async Task SetRoomSettings(RoomSettings settings, Room room, Participant participant)
    {
        if (await room.SetRoomSettings(settings, participant))
        {
            await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomStateChanged", room.ToRoomStateDto());
        }
    }

    internal Room? GetRoom(Participant? participant)
    {
        if (participant is null)
        {
            return null;
        }

        return _participantToRoomsDictionary.TryGetValue(participant, out var room) 
            ? room 
            : null;
    }

    internal Room? GetRoom(String roomName)
    {
        lock (_rooms)
        {
            return _rooms.FirstOrDefault(r => r.RoomName.Equals(roomName));
        }
    }
    internal async Task<bool> CreateRoomAsync(String roomName, RoomSettings roomSettings,
        CancellationToken cancellationToken = default)
    {
        lock (_rooms)
        {
            if (String.IsNullOrWhiteSpace(roomName)
                || _rooms.Any(r => r.RoomName.Equals(roomName, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
        }

        var newRoom = new PublicRoom(_hubContext, roomName, roomSettings, EventEndedAsync);
        AddRoom(newRoom);
        
        await _hubContext.Clients
            .Group(_lobbyGroupName)
            .SendAsync("RoomCreated", newRoom.ToRoomStateDto(), cancellationToken: cancellationToken);
        
        return true;
    }

    internal async Task LeaveRoomAsync(Participant participant, Room room,
        CancellationToken cancellationToken = default)
    {
        if (_participantToRoomsDictionary.Remove(participant))
        {
            await room.RemoveParticipantAsync(participant, cancellationToken);
            if (!room.Participants.Any())
            {
                lock (_rooms)
                {
                    _rooms.Remove(room);
                }
                await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomDeleted", room.ToRoomStateDto(), cancellationToken: cancellationToken);
            }

            await _hubContext.Clients.GroupExcept(room.RoomName, participant.ConnectionId)
                .SendAsync("ParticipantLeft", participant.ToDTO(), cancellationToken: cancellationToken);
            await _hubContext.Groups.RemoveFromGroupAsync(participant.ConnectionId, room.RoomName,
                cancellationToken: cancellationToken);
            await _hubContext.Clients.Group(_lobbyGroupName).SendAsync("RoomStateChanged", room.ToRoomStateDto(),
                cancellationToken: cancellationToken);
        }
    }

    private async Task EventEndedAsync(Room room, CancellationToken cancellationToken = default)
    {
        var disconnectedParticipants = Enumerable.Empty<Participant>().ToList();
        
        disconnectedParticipants.AddRange(room.Participants.Where(p => !p.IsConnected));

        foreach (var participant in disconnectedParticipants)
        {
            await LeaveRoomAsync(participant, room, cancellationToken);
        }
    }
}
