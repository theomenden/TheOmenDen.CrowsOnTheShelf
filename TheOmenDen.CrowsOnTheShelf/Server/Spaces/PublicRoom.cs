using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsOnTheShelf.Server.Hubs;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

public sealed class PublicRoom: Room
{
    public PublicRoom(IHubContext<SecretSantaHub> context, String roomName, RoomSettings settings, Func<Room, CancellationToken, Task> eventEndedCallback)
        :base(context, roomName, settings, eventEndedCallback)
    {
        
    }
}
