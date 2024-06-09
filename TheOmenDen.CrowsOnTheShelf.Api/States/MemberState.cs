using Ardalis.SmartEnum;

namespace TheOmenDen.CrowsOnTheShelf.Api.States;

public abstract class MemberState : SmartEnum<MemberState>
{
    public static readonly MemberState Active = new ActiveMemberState();
    public static readonly MemberState Inactive = new InactiveMemberState();
    public static readonly MemberState Pending = new PendingMemberState();
    public static readonly MemberState Verified = new VerifiedMemberState();

    private MemberState(string name, int value) : base(name, value) { }

    private sealed class ActiveMemberState() : MemberState(nameof(Active), 1);

    private sealed class InactiveMemberState() : MemberState(nameof(Inactive), 2);

    private sealed class PendingMemberState() : MemberState(nameof(Pending), 3);

    private sealed class VerifiedMemberState() : MemberState(nameof(Verified), 4);
}