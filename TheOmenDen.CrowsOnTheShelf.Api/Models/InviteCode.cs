using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models;

public sealed class InviteCode
{
    public InviteCodeId Id { get; set; } = InviteCodeId.New();
    public string Code { get; set; }
    public GiftGroupId GroupId { get; set; }
    public ICollection<string> AuthorizedEmails = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
    public int UsesLeft { get; set; } = 10;
    public GiftGroup Group { get; set; }
}