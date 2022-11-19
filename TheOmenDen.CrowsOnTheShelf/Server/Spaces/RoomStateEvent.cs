using System.Timers;
using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

internal sealed class RoomStateEvent: IRoomState
{
    private Int32 _entryCount = 0;
    private readonly Room _room;
    private readonly RoomStateLobby _roomStateLobby;
    private Dictionary<Participant, bool> _participantGiftsBought= new ();
    private System.Timers.Timer? _eventEndTimer;

    public RoomStateEvent(Room room, RoomStateLobby rsl)
    {
            _room = room;
            _roomStateLobby = rsl;
    }

    public async Task EnterAsync(CancellationToken cancellationToken = default)
    {
        if (_entryCount is 0)
        {
            await _room.SendAll("EventStarted");
        }

        if(_room.Participants.Count(p => p.IsConnected) is 0)
        {
            _room.RoomState = _roomStateLobby;
            return;
        }

        var timeout = 12 + (2 * _participantGiftsBought.Count);
        _eventEndTimer = new System.Timers.Timer(timeout * 1000);
        _eventEndTimer.Elapsed += EventEndTimerElapsed;
        _eventEndTimer.AutoReset = false;
        _eventEndTimer.Start();
        _entryCount++;
    }

    public async Task AddParticipantAsync(Lobbies.Participant participant, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _room.SendParticipant(participant, "EventStarted");
    }

    public Task RemovePlayerAsync(Lobbies.Participant participant, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    internal Task AddGiftsBought(List<(Participant participant, bool hasGiftBeenBought)> gifts, CancellationToken cancellationToken = default)
    {
        lock (_participantGiftsBought)
        {
            foreach (var (participant, hasGiftBeenBought) in gifts)
            {
                if (_participantGiftsBought.TryGetValue(participant, out var _))
                {
                    _participantGiftsBought.Remove(participant);
                    _participantGiftsBought.Add(participant, hasGiftBeenBought);
                    continue;
                }

                _participantGiftsBought.Add(participant, hasGiftBeenBought);
            }
        }

        return SendGiftsBoughtAsync(cancellationToken);
    }

    private Task SendGiftsBoughtAsync(CancellationToken cancellationToken = default)
    {
        var totalGifts = GetTotalGifts().ToList();

        return _room.SendAll("UpdateGiftsBought", totalGifts);
    }

    private IEnumerable<ParticipantGift> GetTotalGifts()
    {
        var totalGifts = Enumerable.Empty<ParticipantGift>();

        lock(_participantGiftsBought)
        {
            totalGifts = _participantGiftsBought.Select(s => new ParticipantGift(s.Key.ToDTO(), s.Value));
        }
        return totalGifts;
    }

    private void EventEndTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _eventEndTimer?.Stop();
        _eventEndTimer?.Dispose();

        _room.RoomState = _roomStateLobby;

        _ = _room.SendAll("EventEnded");
    }
}
