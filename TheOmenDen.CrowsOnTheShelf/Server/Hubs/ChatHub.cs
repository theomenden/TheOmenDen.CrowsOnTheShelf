using Microsoft.AspNetCore.SignalR;

namespace TheOmenDen.CrowsOnTheShelf.Server.Hubs;

public class ChatHub: Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
