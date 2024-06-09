using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;
using TheOmenDen.CrowsOnTheShelf.Api.States;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models;

public sealed class GiftGroupMember
{
    public GroupMemberId GroupMemberId { get; set; } = GroupMemberId.New();
    public GiftGroupId GiftGroupId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsOrganizer { get; set; } = false;
    public MemberState MemberState { get; set; } = MemberState.Pending;
    public GiftGroup Group { get; set; }
}