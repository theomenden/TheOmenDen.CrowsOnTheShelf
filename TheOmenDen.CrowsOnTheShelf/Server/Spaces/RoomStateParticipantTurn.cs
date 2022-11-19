using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Enumerations;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

internal sealed class RoomStateParticipantTurn : IRoomState
{
    private readonly Participant _participant;
    private readonly Room _room;
    private readonly RoomStateRound _roomRoundState;
    private int _giftBoughtCount = 0;

    public RoomStateParticipantTurn(Participant participant, Room room, RoomStateRound roomStateRound)
    {
        _participant = participant;
        _room = room;
        _roomRoundState = roomStateRound;
    }

    public List<(Participant participant, bool hasPurchasedGift)> Gifts { get; internal set; } = new(50);

    public async Task EnterAsync(CancellationToken cancellationToken = default)
    {
        switch (_giftBoughtCount)
        {
            case 0:
                await StartParticipantTurnAsync(cancellationToken);
                break;
            case 1:
                await EndParticipantTurnAsync(cancellationToken);
                break;
            default: throw new InvalidOperationException($"Unknown Gifts Bought: {_giftBoughtCount}.");
        }
        _giftBoughtCount++;
    }

    public async Task AddParticipantAsync(Participant participant, bool isReconnection, CancellationToken cancellationToken = default)
    {
        await _roomRoundState.AddParticipantAsync(participant, isReconnection, cancellationToken);
        await _room.SendParticipant(participant, "ChatMessage",
            new ChatMessage(ChatMessageType.EventFlow, null, $"{_participant.Name} still needs a gift!"));
    }

    public Task RemovePlayerAsync(Participant participant, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    private async Task EndParticipantTurnAsync(CancellationToken cancellationToken = default)
    {
        await _roomRoundState.AddGiftBoughtAsync(Gifts, cancellationToken);
        _room.RoomState = _roomRoundState;
    }

    private async Task StartParticipantTurnAsync(CancellationToken cancellationToken = default)
    {
        await _room.SendAll("ChatMessage",
            new ChatMessage(ChatMessageType.EventFlow, null, $"{_participant.Name} still needs a gift!"));

        _room.RoomState = new RoomStateGiftBought(_participant, _room, this);
    }

}
