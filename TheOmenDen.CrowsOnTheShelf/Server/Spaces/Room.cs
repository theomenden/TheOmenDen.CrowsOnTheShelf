using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsOnTheShelf.Server.Hubs;
using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

public class Room
{
    private static int RoomCounter = 0;

    private int _roomIndex;
    private List<Participant> _participants = new(10);
    private IHubContext<SecretSantaHub> _hubContext;
    private IRoomState _roomState;

    public Room(IHubContext<SecretSantaHub> context, string name, Func<Room, Task> eventEndedCallback)
        :this(context, name, new RoomSettings(), eventEndedCallback)
    {
        
    }

    public Room(IHubContext<SecretSantaHub> context, string name, RoomSettings settings, Func<Room, Task> eventEndedCallback)
    {
        _roomIndex = RoomCounter++;
        _hubContext = context;
        RoomName = name;
        RoomSettings = settings;
        _roomState = new RoomStateLobby(this, eventEndedCallback);
        _ = _roomState.EnterAsync();
    }

    internal int RoomIndex => _roomIndex;

    internal IHubContext<SecretSantaHub> HubContext => _hubContext;

    public List<Participant> Participants
    {
        get
        {
            lock (_participants)
            {
                return _participants.ToList();
            }
        }
    }

    public String RoomName { get; }

    public RoomSettings RoomSettings { get; internal set; }

    public Boolean IsEventInProgress => _roomState is not RoomStateLobby;

    public async Task AddParticipantAsync(Participant participant, bool isReconnection = false,
        CancellationToken cancellationToken = default)
    {
        if(!isReconnection)
        {
            lock (_participants)
            {
                _participants.Add(participant);
            }
        }

        await _roomState.AddParticipantAsync(participant, isReconnection, cancellationToken);
    }

    public async Task<bool> RemoveParticipantAsync(Participant participant,
        CancellationToken cancellationToken = default)
    {
        bool isParticipantRemoved;
        lock (_participants)
        {
            isParticipantRemoved = _participants.Remove(participant);
        }

        if (!isParticipantRemoved)
        {
            return false;
        }

        await _roomState.RemovePlayerAsync(participant, cancellationToken);

        return true;
    }

    internal IRoomState RoomState
    {
        get => _roomState;
        set
        {
            if (value is null || _roomState == value)
            {
                return;
            }

            _roomState= value;
            _ = _roomState.EnterAsync();
        }
    }

    internal RoomStateDTO ToRoomStateDto()
    {
        var participantDtos = Enumerable.Empty<ParticipantDto>().ToList();
        
        lock (_participants)
        {
            participantDtos = _participants.Select(p => p.ToDTO()).ToList();
        }

        return new(RoomName, participantDtos, RoomSettings, IsEventInProgress);
    }

    #region SignalR Group Send Methods
    internal Task SendAll(String method)
    {
        return HubContext.Clients.Group(RoomName).SendAsync(method);
    }
    internal Task SendAll(String method, object? arg)
    {
        return HubContext.Clients.Group(RoomName).SendAsync(method, arg);
    }
    internal Task SendAll(String method, object? arg1, object? arg2)
    {
        return HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2);
    }
    internal Task SendAll(String method, object? arg1, object? arg2, object? arg3)
    {
        return HubContext.Clients.Group(RoomName).SendAsync(method, arg1, arg2, arg3);
    }
    internal Task SendAllExcept(Participant contact, String method)
    {
        return HubContext.Clients.GroupExcept(RoomName, contact.ConnectionId).SendAsync(method);
    }
    internal Task SendAllExcept(Participant contact, String method, object? arg)
    {
        return HubContext.Clients.GroupExcept(RoomName, contact.ConnectionId).SendAsync(method, arg);
    }
    internal Task SendAllExcept(Participant contact, String method, object? arg1, object? arg2)
    {
        return HubContext.Clients.GroupExcept(RoomName, contact.ConnectionId).SendAsync(method, arg1, arg2);
    }
    internal Task SendAllExcept(Participant contact, String method, object? arg1, object? arg2, object arg3)
    {
        return HubContext.Clients.GroupExcept(RoomName, contact.ConnectionId).SendAsync(method, arg1, arg2, arg3);
    }
    internal Task SendAllExcept(Participant contact, String method, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        return HubContext.Clients.GroupExcept(RoomName, contact.ConnectionId).SendAsync(method, arg1, arg2, arg3, arg4);
    }
    internal Task SendParticipant(Participant contact, String method)
    {
        return HubContext.Clients.Client(contact.ConnectionId).SendAsync(method);
    }
    internal Task SendParticipant(Participant contact, String method, object? arg)
    {
        return HubContext.Clients.Client(contact.ConnectionId).SendAsync(method, arg);
    }
    internal Task SendParticipant(Participant contact, String method, object? arg1, object? arg2)
    {
        return HubContext.Clients.Client(contact.ConnectionId).SendAsync(method, arg1, arg2);
    }
    internal Task SendParticipant(Participant contact, String method, object? arg1, object? arg2, object? arg3)
    {
        return HubContext.Clients.Client(contact.ConnectionId).SendAsync(method, arg1, arg2, arg3);
    }
    internal Task SendParticipant(Participant contact, String method, object? arg1, object? arg2, object? arg3, object? arg4)
    {
        return HubContext.Clients.Client(contact.ConnectionId).SendAsync(method, arg1, arg2, arg3, arg4);
    }
    #endregion
}
