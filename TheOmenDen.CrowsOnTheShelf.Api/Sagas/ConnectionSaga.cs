using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsOnTheShelf.Api.Hubs;
using TheOmenDen.CrowsOnTheShelf.Api.Services;

namespace TheOmenDen.CrowsOnTheShelf.Api.Sagas;

public interface IConnectionSaga
{
    Task ExecuteSagaAsync(string inviteCode, string email, string connectionId, CancellationToken cancellationToken = default);
}

[RegisterService<IConnectionSaga>(LifeTime.Scoped)]
internal sealed class ConnectionSaga(ILogger<ConnectionSaga> logger, IInviteCodeValidationService inviteCodeValidationService, IHubContext<NotificationHub, INotificationClient> hubContext)
: IConnectionSaga
{
    public async Task ExecuteSagaAsync(string inviteCode, string email, string connectionId, CancellationToken cancellationToken = default)
    {
        var isCodeValid = await inviteCodeValidationService.ValidateInviteCodeAsync(inviteCode, email);

        if (!isCodeValid)
        {
            await hubContext.Clients.Client(connectionId).NotifyJoinAttemptFailed("Invalid invite code or email.", cancellationToken);
            return; // Compensate if necessary
        }

        try
        {
            await hubContext.Groups.AddToGroupAsync(connectionId, inviteCode, cancellationToken);
            await hubContext.Clients.Group(inviteCode).NotifyJoinAttemptSuccess(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to join group.");
            // Compensatory action for failure after code validation
            await hubContext.Clients.Client(connectionId).NotifyJoinAttemptFailed("Failed to join group.", cancellationToken);
        }
    }
}
