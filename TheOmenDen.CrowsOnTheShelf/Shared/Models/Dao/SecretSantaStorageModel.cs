namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
public sealed class SecretSantaStorageModel : IEquatable<SecretSantaStorageModel>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime OccurringAt { get; set; }

    public String Code { get; set; }

    public Decimal Budget { get; set; }

    public IEnumerable<String> ParticipantEmails { get; set; } = Enumerable.Empty<String>();

    public IEnumerable<GifterStorageModel> Gifters { get; set; } = Enumerable.Empty<GifterStorageModel>();

    public IEnumerable<RecipientStorageModel> Recipients { get; set; } = Enumerable.Empty<RecipientStorageModel>();

    public bool HaveAllParticipantsAccepted() => ParticipantEmails.Any();

    public bool Equals(SecretSantaStorageModel? other)
    => other is not null
       && (ReferenceEquals(this, other)
            || Id.Equals(other.Id)
            && Code == other.Code);
    

    public override bool Equals(object? obj) => obj is not null &&
                                                ReferenceEquals(this, obj)
                                                || obj is SecretSantaStorageModel other && Equals(other);


    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Code);
    }

    public static bool operator ==(SecretSantaStorageModel? left, SecretSantaStorageModel? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SecretSantaStorageModel? left, SecretSantaStorageModel? right)
    {
        return !Equals(left, right);
    }
}
