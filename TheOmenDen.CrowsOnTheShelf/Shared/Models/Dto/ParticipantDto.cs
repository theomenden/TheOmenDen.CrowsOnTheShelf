namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
public sealed class ParticipantDto: IEquatable<ParticipantDto>
{
    public ParticipantDto(String name, Guid id, Boolean isConnected)
    {
        
    }

    public Guid Id { get; set; }

    public String Name { get; set; }

    public bool IsConnected { get; set; }


    public bool Equals(ParticipantDto? other)
    => other is not null && other.Id == Id;

    public override Boolean Equals(Object? obj) =>
        obj is not null 
        && (ReferenceEquals(this, obj) 
            || obj is ParticipantDto other 
            && Equals(other));

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, IsConnected);
    }

    public static bool operator ==(ParticipantDto? left, ParticipantDto? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ParticipantDto? left, ParticipantDto? right)
    {
        return !Equals(left, right);
    }
}
