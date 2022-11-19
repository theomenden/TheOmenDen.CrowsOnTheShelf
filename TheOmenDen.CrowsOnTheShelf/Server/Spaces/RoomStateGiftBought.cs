using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

internal sealed class RoomStateGiftBought: IRoomState
{
    private readonly Participant _activeParticipant;
    private readonly Room _room;
    private readonly RoomStateParticipantTurn _roomStateParticipantTurn;
    private readonly EventTimer _eventTimer;

    public RoomStateGiftBought(Participant participant, Room room, RoomStateParticipantTurn roomStateParticipantTurn)
    {
        _activeParticipant= participant;
        _room= room;
        _roomStateParticipantTurn= roomStateParticipantTurn;
    }

    public async Task EnterAsync(CancellationToken cancellationToken = default)
    {
        await _room.SendParticipant(_activeParticipant, "ActiveParticipantGiftBuying");
    }

    public Task AddParticipantAsync(Participant participant, bool isReconnection, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemovePlayerAsync(Participant participant, CancellationToken cancellationToken = default)
    {
        if (participant == _activeParticipant)
        {
            _room.RoomState = _roomStateParticipantTurn;
        }

        return Task.CompletedTask;
    }
}
