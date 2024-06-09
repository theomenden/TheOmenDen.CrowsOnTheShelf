using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsOnTheShelf.Api.Sagas;
using TheOmenDen.CrowsOnTheShelf.Api.Services;

namespace TheOmenDen.CrowsOnTheShelf.Api.Hubs;

public interface INotificationClient
{
    Task MemberJoined(string message, CancellationToken cancellationToken = default);
    Task PairingUpdated(string message, CancellationToken cancellationToken = default);
    Task StateChanged(string newState, CancellationToken cancellationToken = default);
    Task NotifyJoinAttemptFailed(string message, CancellationToken cancellationToken = default);
    Task NotifyJoinAttemptSuccess(string email, CancellationToken cancellationToken = default);
}

public class NotificationHub(IConnectionSaga connectionSaga) : Hub<INotificationClient>
{
    public async Task NotifyMemberJoined(string groupId, string email)
    {
        // Compose the message to be sent to clients
        string message = $"New member with email {email} has joined the group.";

        // Send the message to all clients in the specified group
        await Clients.Group(groupId).MemberJoined(message);
    }

    public async Task NotifyPairingUpdated(string groupId)
    {
        string message = "Pairing has been updated.";
        await Clients.Group(groupId).PairingUpdated(message);
    }

    public async Task NotifyStateChanged(string groupId, string newState)
    {
        await Clients.Group(groupId).StateChanged(newState);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var inviteCode = httpContext.Request.Query["inviteCode"];
        var email = httpContext.Request.Query["email"];

        await connectionSaga.ExecuteSagaAsync(inviteCode.ToString(), email.ToString(), Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // You can remove clients from groups or perform other cleanup
        // await Groups.RemoveFromGroupAsync(Context.ConnectionId, "someGroupId");

        await base.OnDisconnectedAsync(exception);
    }
}

