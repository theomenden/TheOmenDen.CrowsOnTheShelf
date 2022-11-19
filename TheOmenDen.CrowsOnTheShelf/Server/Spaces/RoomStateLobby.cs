using Microsoft.AspNetCore.Razor.TagHelpers;
using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

public class RoomStateLobby: IRoomState
{
    private readonly Room _room;
    private readonly Func<Room, CancellationToken, Task> _eventEndedCallback;
    private Int32 _entryCount = 0;

    internal RoomStateLobby(Room room, Func<Room, CancellationToken, Task> eventEndedCallback)
    {
            _room= room;
            _eventEndedCallback = eventEndedCallback;
    }

    public async Task EnterAsync(CancellationToken cancellationToken = default)
    {
        if (_entryCount > 0)
        {
            await _eventEndedCallback(_room, cancellationToken);
        }
        _entryCount++;
    }

    public Task AddParticipantAsync(Participant participant, bool isReconnection, CancellationToken cancellationToken = default)
    {
        return _room.SendParticipant(participant, "RoomStateChanged", _room.ToRoomStateDto());
    }

    public Task RemovePlayerAsync(Participant participant, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    internal async Task<Boolean> SetRoomSettingsAsync(RoomSettings settings, Participant participant)
    {
        var firstParticipant = _room.Participants.FirstOrDefault();

        if (firstParticipant is null || firstParticipant != participant)
        {
            return false;
        }

        _room.RoomSettings = settings;
        await _room.SendAllExcept(participant, "RoomStateChanged", _room.ToRoomStateDto());
        return true;
    }
}
