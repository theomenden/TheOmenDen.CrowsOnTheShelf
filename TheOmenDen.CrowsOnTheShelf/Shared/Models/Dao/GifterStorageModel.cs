namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
#nullable disable
public sealed class GifterStorageModel: Entity, IEquatable<GifterStorageModel>
{
    public string FirstName { get; set; } = String.Empty;

    public string Email { get; set; } = String.Empty;

    public Guid RecipientId { get; set; }

    public bool Equals(GifterStorageModel other) => 
        other is not null 
        && (ReferenceEquals(this, other)
            || Id == other.Id
            || Email == other.Email);

    public override bool Equals(object obj) => 
        ReferenceEquals(this, obj) 
        || obj is GifterStorageModel other 
        && Equals(other);

    public override int GetHashCode()
    {
        return (Email != null ? Email.GetHashCode() : 0);
    }

    public static bool operator ==(GifterStorageModel left, GifterStorageModel right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(GifterStorageModel left, GifterStorageModel right)
    {
        return !Equals(left, right);
    }
}
