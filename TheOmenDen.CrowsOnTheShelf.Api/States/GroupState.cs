using Ardalis.SmartEnum;
using Microsoft.AspNetCore.SignalR;
using TheOmenDen.CrowsOnTheShelf.Api.Models;

namespace TheOmenDen.CrowsOnTheShelf.Api.States;

public abstract class GroupState : SmartEnum<GroupState>
{
    public static readonly GroupState Pending = new PendingType();
    public static readonly GroupState Created = new CreatedType();
    public static readonly GroupState InviteCodeGenerated = new InviteCodeGeneratedType();
    public static readonly GroupState Joinable = new JoinableType();
    public static readonly GroupState Ready = new ReadyType();
    public static readonly GroupState Suggestion = new SuggestionType();
    public static readonly GroupState Pairing = new PairingType();
    public static readonly GroupState Paired = new PairedType();
    public static readonly GroupState InProgress = new InProgressType();
    public static readonly GroupState Completed = new CompletedType();
    public static readonly GroupState Expired = new ExpiredType();
    public static readonly GroupState Cancelled = new CancelledType();
    public static readonly GroupState Error = new ErrorType();

    private GroupState(string name, int value) : base(name, value) { }

    public abstract void HandleState(GiftGroup group);

    private sealed class PendingType() : GroupState("Pending", 1)
    {
        public override void HandleState(GiftGroup group)
        {
            // Write a sample case that a group is in pending state when no invite code is present
            group.State = group.InviteCode is null
                ? Created
                : InviteCodeGenerated;
        }
    }

    private sealed class CreatedType() : GroupState("Created", 2)
    {
        public override void HandleState(GiftGroup group)
        {
            /* Initial creation logic here */
            group.State = Pending;
        }
    }

    private sealed class InviteCodeGeneratedType() : GroupState("Invite Code Generated", 3)
    {
        public override void HandleState(GiftGroup group)
        {
            /* Invite code generation logic here */
            group.State = Joinable;
        }
    }

    private sealed class JoinableType() : GroupState("Joinable", 4)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is joinable */ }

        public async Task OnMemberJoinAsync(GiftGroup group, GroupMember newMember, IHubContext<NotificationHub> hubContext)
        {
            // Logic to handle a new member joining the group
            group.Members.Add(newMember);

            // Asynchronously notify all clients that a new member has joined
            await hubContext.Clients.Group(group.GroupId.ToString()).SendAsync("MemberJoined", new { MemberEmail = newMember.Email });
        }
    }

    private sealed class ReadyType() : GroupState("Ready", 5)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is ready */ }
    }

    private sealed class SuggestionType() : GroupState("Suggestion", 6)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is in suggestion state */ }
    }

    private sealed class PairingType() : GroupState("Pairing", 7)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is in pairing state */ }
    }

    private sealed class PairedType() : GroupState("Paired", 8)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is paired */ }
    }

    private sealed class InProgressType() : GroupState("In Progress", 9)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is in progress */ }
    }

    private sealed class CompletedType() : GroupState("Completed", 10)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is completed */ }
    }

    private sealed class ExpiredType() : GroupState("Expired", 11)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is expired */ }
    }

    private sealed class CancelledType() : GroupState("Cancelled", 12)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is cancelled */ }
    }

    private sealed class ErrorType() : GroupState("Error", 13)
    {
        public override void HandleState(GiftGroup group) { /* Logic when group is in error state */ }
    }
}