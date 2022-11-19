using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Resources;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Enumerations;

namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

internal sealed class SecretSantaEventService: ISecretSantaEventService
{
    private HubConnection _hubConnection;
    private NavigationManager _navigationManager;

    
    private EventState _eventState = new ();
    private List<RoomStateDTO> _rooms = new(10);
    private List<Participant> _participants= new(20);

    private RoomStateDTO? _currentRoomState = null;
    private Guid? _participantGuid = null;

    public event EventHandler? RoomListChanged;
    public event EventHandler? ParticipantListChanged;
    public event EventHandler? RoomSettingsChanged;
    public event EventHandler? EventStarted;
    
    public SecretSantaEventService(NavigationManager navigationManager)
    {
        _navigationManager= navigationManager;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/game"))
            .Build();

        _= InitializeEvent();
    }

    private async Task InitializeEvent()
    {
        InitializeServerCallbacks();

        await _hubConnection.StartAsync();
        var newRooms = await _hubConnection.InvokeAsync<List<RoomStateDTO>>("GetRooms");
        AddRooms(newRooms);
    }

    private void InitializeServerCallbacks()
    {
        _hubConnection.On<ParticipantDto>("ParticipantJoined", (p) =>
        {
            var participant = new Participant(p);

            if (_participants.Contains(participant))
            {
                return;
            }

            _participants.Add(participant);
            ParticipantListChanged?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On<ParticipantDto>("ParticipantConnectionStatusChanged", p =>
        {
            var participant = _participants.SingleOrDefault(p => p.Id.Equals(p.Id));

            if (participant == null)
            {
                return;
            }

            participant.IsConnected = p.IsConnected;
            ParticipantListChanged?.Invoke(this, EventArgs.Empty);

            if (p.IsConnected)
            {
                _eventState.AddChatMessage(new(ChatMessageType.EventFlow, p.Name,
                    $"{p.Name} reconnected."));
                return;
            }

            _eventState.AddChatMessage(new(ChatMessageType.EventFlow, p.Name,
                $"{p.Name} disconnected"));
        });

        _hubConnection.On<ParticipantDto>("ParticipantLeft", (p) =>
        {
            var participant = new Participant(p);

            if (!_participants.Contains(participant))
            {
                return;
            }

            _participants.Remove(participant);
            ParticipantListChanged?.Invoke(this, EventArgs.Empty);
            _eventState.AddChatMessage(new(ChatMessageType.EventFlow, p.Name, $"{p.Name} left the game"));
        });

        _hubConnection.On<RoomStateDTO>("RoomCreated", AddRoom);
        _hubConnection.On<RoomStateDTO>("RoomDeleted", RemoveRoom);

        _hubConnection.On<RoomStateDTO>("RoomStateChanged", (state) =>
        {
            if (_currentRoomState is not null && state.RoomName.Equals(_currentRoomState.RoomName))
            {
                _currentRoomState = state;
            }

            var room = _rooms.FirstOrDefault(r => r.RoomName.Equals(state.RoomName));

            if (room != null)
            {
                room.IsGameInProgress = state.IsGameInProgress;
                room.RoomSettings= state.RoomSettings;
                room.Participants= state.Participants;
            }

            RoomSettingsChanged?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On("EventStarted", () =>
        {
            foreach (var participant in _participants)
            {
                participant.IsGiftPurchased = false;
                participant.Position = null;
            }

            EventStarted?.Invoke(this, EventArgs.Empty);
        });

        _hubConnection.On<int,int,ChatMessage>("RoundStarted", (currentRound, roundCount, chatMessage) => 
            _eventState.NewRoundStart(currentRound, roundCount, chatMessage));
        
        _hubConnection.On<ChatMessage>("ChatMessage", (cm) => _eventState.AddChatMessage(cm));

        _hubConnection.On("EventEnded", () => _navigationManager.NavigateTo("/waitingRoom"));
    }

    public IEnumerable<RoomStateDTO> Rooms => _rooms;

    public IEnumerable<Participant> Participants => _participants;

    public RoomStateDTO? RoomState => _currentRoomState;

    public EventState EventState => _eventState;

    public Guid? ParticipantGuid => _participantGuid;

    public async Task<RoomStateDTO?> TryReconnect(String userName, Guid connectionId,
        CancellationToken cancellationToken = default)
    {
        var reconnectState = await _hubConnection.InvokeAsync<ReconnectionStateDto>("TryReconnect", userName, connectionId, cancellationToken: cancellationToken);

        if (reconnectState.RoomState != null)
        {
            _participantGuid = reconnectState.ParticipantId;
            _participants = reconnectState.RoomState.Participants.Select((p) => new Participant(p)).ToList();

            ParticipantListChanged?.Invoke(this, EventArgs.Empty);

            _currentRoomState = reconnectState.RoomState;
            RoomSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        return RoomState;
    }

    public async Task<bool> CreateRoom(String roomName, RoomSettings roomSettings,
        CancellationToken cancellationToken = default)
    => await _hubConnection.InvokeAsync<bool>("CreateRoom", roomName, roomSettings, cancellationToken: cancellationToken);

    public async Task<bool> JoinRoom(String roomName, String roomCode, CancellationToken cancellationToken = default)
    {
        var roomState = await _hubConnection.InvokeAsync<RoomStateDTO>("JoinRoom", roomName, roomCode, cancellationToken: cancellationToken);

        if (roomState is null)
        {
            _participants = new List<Participant>();
            ParticipantListChanged?.Invoke(this, EventArgs.Empty);
            _currentRoomState = null;
            RoomSettingsChanged?.Invoke(this, EventArgs.Empty);
            return false;
        }

        _participants = roomState.Participants.Select((p) => new Participant(p)).ToList();
        ParticipantListChanged?.Invoke(this, EventArgs.Empty);
        _currentRoomState = roomState;
        RoomSettingsChanged?.Invoke(this, EventArgs.Empty);

        if (roomState.IsGameInProgress)
        {
            _navigationManager.NavigateTo("/room");

            return true;
        }

        _navigationManager.NavigateTo("/waitingRoom");
        return true;
    }

    public async Task<bool> LeaveRoom(CancellationToken cancellationToken = default)
    {
        if (!await _hubConnection.InvokeAsync<bool>("LeaveRoom", cancellationToken: cancellationToken))
        {
            return false;
        }

        var newRooms = await _hubConnection.InvokeAsync<List<RoomStateDTO>>("GetRooms", cancellationToken: cancellationToken);

        _rooms.Clear();

        AddRooms(newRooms);
        return true;

    }

    public Task<bool> StartEvent(CancellationToken cancellationToken = default) =>
        _hubConnection.InvokeAsync<bool>("StartGame", cancellationToken: cancellationToken);

    public Task SetRoomSettings(RoomSettings roomSettings)
    {
        if (_currentRoomState is not null)
        {
            _currentRoomState.RoomSettings = roomSettings;
        }

        return _hubConnection.InvokeAsync("SetRoomSettings", roomSettings);
    }

    public async Task<Guid> SetParticipantName(String userName, CancellationToken cancellationToken = default)
    {
        var participantGuids = await _hubConnection.InvokeAsync<ParticipantGuids>("SetParticipantName", userName,
            cancellationToken: cancellationToken);

        _participantGuid = participantGuids.PlayerId;

        return participantGuids.ConnectionId;
    }

    private void AddRooms(IEnumerable<RoomStateDTO> newRooms)
    {
        _rooms.AddRange(newRooms);
        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void AddRoom(RoomStateDTO newRoom)
    {
        _rooms.Add(newRoom);
        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveRoom(RoomStateDTO room)
    {
        var room2 = _rooms.FirstOrDefault(r => r.RoomName.Equals(room.RoomName));

        if (room2 == null)
        {
            return;
        }

        _rooms.Remove(room2);
        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _eventState?.TurnTimer?.Dispose();
    }
}
