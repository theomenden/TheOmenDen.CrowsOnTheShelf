using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsOnTheShelf.Api.Hubs;
using TheOmenDen.CrowsOnTheShelf.Api.Models.Requests;
using TheOmenDen.CrowsOnTheShelf.Api.Services;

namespace TheOmenDen.CrowsOnTheShelf.Api.Sagas;

public interface IJoinGroupSaga
{
    Task<bool> ExecuteSagaAsync(JoinGroupRequest request, CancellationToken cancellationToken);
}

internal sealed class JoinGroupSaga(ILogger<JoinGroupSaga> logger, IGiftGroupService groupService, IInviteCodeValidationService inviteCodeValidation, IHubContext<NotificationHub, INotificationClient> hubContext)
: IJoinGroupSaga
{

    public async Task<bool> ExecuteSagaAsync(JoinGroupRequest request, CancellationToken cancellationToken)
    {
        // Validate the invite code first
        var inviteCodeValid = await inviteCodeValidation.ValidateInviteCodeAsync(request.InviteCode, request.Email, cancellationToken);
        if (!inviteCodeValid)
        {
            await hubContext.Clients.Client(request.ConnectionId).NotifyJoinAttemptFailed("Invalid or expired invite code.");
            return false;
        }

        // Add user to the group
        var joinSuccess = await groupService.JoinGroupAsync(request.Email, request.InviteCode, cancellationToken);
        if (!joinSuccess)
        {
            await hubContext.Clients.Client(request.ConnectionId).NotifyJoinAttemptFailed("Failed to add member to group.");
            return false;
        }

        // Notify all group members
        await hubContext.Clients.Group(request.GroupId.ToString()).MemberJoined($"{request.Email} has joined the group.");

        return true;
    }
}
