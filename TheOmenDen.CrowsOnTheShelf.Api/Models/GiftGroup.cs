using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;
using TheOmenDen.CrowsOnTheShelf.Api.States;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models;

public class GiftGroup
{
    public GiftGroupId Id { get; set; } = GiftGroupId.New();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime EventTakesPlaceAt { get; set; } = DateTime.UtcNow.AddDays(7);
    public GroupState State { get; set; } = GroupState.;
    public InviteCode? InviteCode { get; set; } = null!;
    public ICollection<GiftGroupMember> Members { get; set; } = new List<GiftGroupMember>();
}