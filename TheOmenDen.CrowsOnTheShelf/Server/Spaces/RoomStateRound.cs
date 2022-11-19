using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Enumerations;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

internal sealed class RoomStateRound: IRoomState
{
    private readonly int _roundNumber;
    private readonly Room _room;
    private readonly RoomStateEvent _roomStateEvent;
    private int _giftsBought = 0;
    private readonly List<Participant> _participantsAlreadyBoughtGifts = new(10);

    public RoomStateRound(Int32 roundNumber, Room room, RoomStateEvent roomStateEvent)
    {
        _roundNumber= roundNumber;
        _room= room;
        _roomStateEvent= roomStateEvent;
    }

    public async Task EnterAsync(CancellationToken cancellationToken = default)
    {
        if (_giftsBought is 0)
        {
            var cm = new ChatMessage(ChatMessageType.EventFlow, null, $"");

            await _room.SendAll("EventStarted", _roundNumber + 1, _room.RoomSettings.GameName, cm);
        }

        var remainingParticipants = _room.Participants.Except(_participantsAlreadyBoughtGifts).Where(p => p.IsConnected)
            .ToArray();

        if (remainingParticipants.Any())
        {
            var participant = remainingParticipants.First();

            _participantsAlreadyBoughtGifts.Add(participant);
            _room.RoomState= new RoomStateParticipantTurn(participant, _room, this);

        }
        else
        {
            _room.RoomState = _roomStateEvent;
        }
        _giftsBought++;
    }

    public async Task AddParticipantAsync(Participant participant, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomStateEvent.AddParticipantAsync(participant, isReconnection, cancellationToken);

        var cm = new ChatMessage(ChatMessageType.EventFlow, null, $"Days {_roundNumber + 1} remaining.");

        await _room.SendParticipant(participant, "RoundStarted", _roundNumber - 1, _room.RoomSettings.Rounds, cm);
    }

    public Task RemovePlayerAsync(Participant participant, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    internal Task AddGiftBoughtAsync(List<(Participant participant, bool hasGiftBeenBought)> gifts, CancellationToken cancellationToken = default)
        => _roomStateEvent.AddGiftsBought(gifts, cancellationToken);
}
