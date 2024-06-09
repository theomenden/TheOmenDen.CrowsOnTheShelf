using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsOnTheShelf.Api.Contexts;
using TheOmenDen.CrowsOnTheShelf.Api.Models;
using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;
using TheOmenDen.CrowsOnTheShelf.Api.States;

namespace TheOmenDen.CrowsOnTheShelf.Api.Services;

public interface IGiftGroupService
{
    Task<GroupDto> CreateGroupAsync(string groupName, IEnumerable<string> authorizedEmails, CancellationToken cancellationToken = default);
    Task<bool> JoinGroupAsync(string email, string inviteCodeStr, CancellationToken cancellationToken = default);
    void TransitionState(GiftGroup group, GroupState newState);
}

[RegisterService<IGiftGroupService>(LifeTime.Scoped)]
internal sealed class GiftGroupService(IDbContextFactory<CrowsOnTheShelfContext> contextFactory, ILogger<GiftGroupService> logger, IInviteCodeGenerationService inviteCodeGenerationService)
: IGiftGroupService
{
    public void TransitionState(GiftGroup group, GroupState newState)
    {
        group.State = newState;
        group.State.HandleState(group);
    }

    public async Task<GroupDto> CreateGroupAsync(string groupName, IEnumerable<string> authorizedEmails, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var group = new GiftGroup { Name = groupName };
        var inviteCode = inviteCodeGenerationService.GenerateInviteCode(authorizedEmails);

        group.InviteCode = inviteCode;
        context.Groups.Add(group);
        await context.SaveChangesAsync(cancellationToken);

        return new(group.Id, group.Name, new(inviteCode.Id, inviteCode.Code));
    }

    public async Task<bool> JoinGroupAsync(string email, string inviteCodeStr, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

        var group = await context.Groups
            .Include(g => g.InviteCode)
            .FirstOrDefaultAsync(g => g.InviteCode.Code == inviteCodeStr, cancellationToken);

        if (group?.InviteCode?.AuthorizedEmails.Contains(email) is not true || !(group?.InviteCode?.UsesLeft > 0))
        {
            return false;
        }

        group.InviteCode.UsesLeft--;
        var groupMember = new GiftGroupMember { Email = email, GiftGroupId = group.Id, MemberState = MemberState.Active };
        context.Members.Add(groupMember);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}