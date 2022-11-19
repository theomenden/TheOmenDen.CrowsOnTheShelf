namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
#nullable disable
public sealed class GiftIdea: Entity
{
    public string Name { get; set; }

    public string Url { get; set; }

    public Guid RecipientId { get; set; }
}
