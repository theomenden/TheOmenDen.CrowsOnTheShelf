using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Graph;
using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Server.Spaces;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
using Participant = TheOmenDen.CrowsOnTheShelf.Server.Lobbies.Participant;

namespace TheOmenDen.CrowsOnTheShelf.Server.Hubs;

public class SecretSantaHub: Hub
{
    public const string HubUrl = "/secretSanta";

    private static readonly Dictionary<String, Participant> ParticipantsDictionary = new(10);

    private readonly Lobby _lobby;

    private readonly ILogger<SecretSantaHub> _logger;

    public SecretSantaHub(Lobby lobby, ILogger<SecretSantaHub> logger)
    {
            _logger= logger;
            _lobby= lobby;
    }

    public override async Task OnConnectedAsync()
    {
        var transportType = Context.Features.Get<IHttpTransportFeature>()?.TransportType;

        _logger.LogDebug("OnConnected SignalR TransportType: {Transport}", transportType);

        Participant? participant = null;
        lock (ParticipantsDictionary)
        {
            if (!ParticipantsDictionary.ContainsKey(Context.ConnectionId))
            {
                participant = new Participant(Context.ConnectionId);
                ParticipantsDictionary.Add(Context.ConnectionId, participant);
            }
        }

        if (participant is not null)
        {
            await _lobby.AddParticipantAsync(participant);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogWarning("Participant disconnected with exception {@Ex}", exception);
        }

        Participant? participant = GetParticipant(Context.ConnectionId);

        if (participant is not null)
        {
            var room = _lobby.GetRoom(participant);

            switch (room)
            {
                case null:
                {
                    await _lobby.RemoveParticipant(participant);
                    lock (ParticipantsDictionary)
                    {
                        ParticipantsDictionary.Remove(Context.ConnectionId);
                    }

                    break;
                }
                default:
                    await _lobby.ParticipantDisconnected(participant, room);
                    break;
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public IEnumerable<RoomStateDTO> GetRooms() => _lobby.GetRooms().ToArray();

    public async Task<ReconnectionStateDto> TryReconnect(String userName, Guid connectionGuid)
    {
        Participant? existingParticipantInstance;

        lock (ParticipantsDictionary)
        {
            existingParticipantInstance = ParticipantsDictionary.Values.SingleOrDefault(p => p.ConnectionGuid== connectionGuid);
        }

        if (existingParticipantInstance is { IsConnected: false })
        {
            lock (ParticipantsDictionary)
            {
                ParticipantsDictionary.Remove(Context.ConnectionId);
                ParticipantsDictionary.Remove(existingParticipantInstance.ConnectionId);
                existingParticipantInstance.ConnectionId= Context.ConnectionId;
                existingParticipantInstance.IsConnected= true;
                ParticipantsDictionary.Add(Context.ConnectionId, existingParticipantInstance);
            }

            var room = _lobby.GetRoom(existingParticipantInstance);

            if (room is { RoomState: not RoomStateLobby })
            {
                await _lobby.ParticipantReconnected(existingParticipantInstance, room);
                return new ReconnectionStateDto(existingParticipantInstance.Id, room.ToRoomStateDto());
            }

            return new ReconnectionStateDto(Guid.Empty, null);
        }

        return new ReconnectionStateDto(Guid.Empty, null);
    }

    public Task<bool> CreateRoom(string roomName, RoomSettings roomSettings)
    => _lobby.CreateRoomAsync(roomName, roomSettings);

    public async Task<RoomStateDTO?> JoinRoom(String roomName, string password)
    {
        var participant = GetParticipant(Context.ConnectionId);

        if (participant is null)
        {
            return null;
        }

        var newRoom = _lobby.GetRoom(roomName);

        return newRoom is not null ? await _lobby.JoinRoomAsync(participant, newRoom, password) : null;
    }

    public async Task<bool> LeaveRoom()
    {
        var participant = GetParticipant(Context.ConnectionId);

        var room = _lobby.GetRoom(participant);

        if (participant is null || room is null)
        {
            return false;
        }

        await _lobby.LeaveRoomAsync(participant, room);
        await _lobby.AddParticipantAsync(participant);

        return true;
    }

    public async Task<bool> StartEvent()
    {
        var participant = GetParticipant(Context.ConnectionId);
        var room = _lobby.GetRoom(participant);

        return room is not null 
               && participant is not null 
               && await _lobby.StartEventAsync(room, participant);
    }

    public ParticipantGuids SetParticipantName(String userName)
    {
        Participant? participant;
        var participantName = userName[..Math.Min(userName.Length, 16)];

        lock (ParticipantsDictionary)
        {
            if (!ParticipantsDictionary.TryGetValue(Context.ConnectionId, out participant))
            {
                participant = new(Context.ConnectionId, participantName);
                ParticipantsDictionary.Add(Context.ConnectionId, participant);
            }
            else
            {
                participant.Name = participantName;
            }
        }

        return new ParticipantGuids(participant.Id, participant.ConnectionGuid);
    }

    public async Task SetRoomSettings(RoomSettings settings)
    {
        var participant= GetParticipant(Context.ConnectionId);
        var room = _lobby.GetRoom(participant);
        
        if (room is not null && participant is not null)
        {
            await _lobby.SetRoomSettings(settings, room,participant);
        }
    }

    private static Participant? GetParticipant(String connectionId)
    {
        lock (ParticipantsDictionary)
        {
            if (ParticipantsDictionary.TryGetValue(connectionId, out var participant))
            {
                return participant;
            }
        }

        return null;
    }
}
