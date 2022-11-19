using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

internal interface ISecretSantaEventService: IDisposable
{
    public event EventHandler? RoomListChanged;
    public event EventHandler? ParticipantListChanged;
    public event EventHandler? RoomSettingsChanged;
    public event EventHandler? EventStarted;

    public IEnumerable<RoomStateDTO> Rooms { get; }
    public IEnumerable<Participant> Participants { get; }
    public RoomStateDTO? RoomState { get; }
    public EventState EventState { get; }
    public Guid? ParticipantGuid { get; }

    public Task<RoomStateDTO?> TryReconnect(String userName, Guid connectionId, CancellationToken cancellationToken = default);
    public Task<Guid> SetParticipantName(String userName, CancellationToken cancellationToken = default);
    public Task<bool> CreateRoom(String roomName, RoomSettings roomSettings, CancellationToken cancellationToken = default);
    public Task<bool> JoinRoom(String roomName, String roomCode, CancellationToken cancellationToken = default);
    public Task SetRoomSettings(RoomSettings roomSettings);
    public Task<bool> StartEvent(CancellationToken cancellationToken = default);
    public Task<bool> LeaveRoom(CancellationToken cancellationToken = default);

}
