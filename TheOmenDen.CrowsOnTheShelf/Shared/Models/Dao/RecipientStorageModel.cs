#nullable disable
namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
public sealed class RecipientStorageModel: Entity, IEquatable<RecipientStorageModel>
{
    public String FirstName { get; set; }

    public String LastName { get; set; }

    public String Email { get; set; }

    public Guid? GifterId { get; set; }
    
    public IEnumerable<GiftIdea> PotentialGifts { get; set; } = Enumerable.Empty<GiftIdea>();

    public bool HasAssociatedGifter() => GifterId.HasValue;

    public bool Equals(RecipientStorageModel other) =>
        other is not null && (ReferenceEquals(this, other) 
                              || Email == other.Email 
                              && Nullable.Equals(GifterId, other.GifterId));

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is RecipientStorageModel other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Email, GifterId);
    }

    public static bool operator ==(RecipientStorageModel left, RecipientStorageModel right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(RecipientStorageModel left, RecipientStorageModel right)
    {
        return !Equals(left, right);
    }
}
