using System.Timers;
using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

internal sealed class RoomStateGifting: IRoomState
{
    private readonly Participant _activeParticipant;
    private readonly Room _room;
    private readonly List<(Participant participant, double remaningTime)> _participantResults;
    private readonly List<Participant> _participants;
    private readonly RoomStateParticipantTurn _roomStateParticipantTurn;
    private readonly EventTimer _eventTimer;
    private readonly int _timeOut;

    private List<(Participant participant, bool hasGiftBeenPurchased)> _participantGifts;

    public RoomStateGifting(Participant participant, Room room, List<Participant> participants, List<(Participant participant, double remaningTime)> participantResults, RoomStateParticipantTurn roomState)
    {
        _activeParticipant = participant;
        _room = room;
        _participants = participants;
        _participantResults = participantResults;
        _roomStateParticipantTurn = roomState;
        CalculatePurchasedGifts();
        _timeOut = 20 + _participantGifts?.Count ?? 0;
        _eventTimer = new EventTimer(_timeOut * 1000, TurnEndTimerElapsed);
    }

    public async Task EnterAsync(CancellationToken cancellationToken = default)
    {
        await SendGiftsPurchased(_timeOut);

        _roomStateParticipantTurn.Gifts = _participantGifts;

        _eventTimer.Start();
    }

    public async Task AddParticipantAsync(Participant participant, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomStateParticipantTurn.AddParticipantAsync(participant, isReconnection, cancellationToken);
        var turnGifts = _participantGifts.Select(s => new ParticipantGift(s.participant.ToDTO(), s.hasGiftBeenPurchased)).ToList();

        var timeRemaining = (int)(_eventTimer.TimeRemaining * 0.001);

        await _room.SendParticipant(participant, "TurnEvents", turnGifts, timeRemaining);
    }

    public Task RemovePlayerAsync(Participant participant, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    private void TurnEndTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _eventTimer.Dispose();
        _room.RoomState = _roomStateParticipantTurn;
    }

    private void CalculatePurchasedGifts()
    {
        _participantGifts = Enumerable.Empty<(Participant, bool)>().ToList();

        foreach (var participant in _participants)
        {
            _participantGifts.Add(new(participant, false));
        }

        if (_participantResults.Count > 0)
        {
            var bestTime = _participantResults[0].remaningTime;

            foreach (var (participant, timeRemaining) in _participantResults)
            {
                var score = (int)(((timeRemaining / bestTime) * 100) + 0.5);

                score = Math.Max(score, 1);
            }
        }

        var totalGiftsCount = _participantGifts.Count + _participantResults.Count;

        if (totalGiftsCount > 0)
        {
            _participantGifts.Add(new(_activeParticipant, true));
            return;
        }

        _participantGifts.Add(new(_activeParticipant, false));

    }

    private Task SendGiftsPurchased(int timeout)
    {
        var giftsPurchasedInTurns = _participantGifts.Select(s => new ParticipantGift(s.participant.ToDTO(), s.hasGiftBeenPurchased));

        return _room.SendAll("TurnGifts", giftsPurchasedInTurns, timeout);
    }
}
