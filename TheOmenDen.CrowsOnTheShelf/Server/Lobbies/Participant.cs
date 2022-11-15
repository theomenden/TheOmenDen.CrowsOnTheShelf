using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Server.Lobbies;

public sealed class Participant : IEquatable<Participant>
{
    public Participant(String connectionId)
    {
        ConnectionId = connectionId;
        Id = Guid.NewGuid();
        ConnectionGuid = Guid.NewGuid();
        IsConnected = true;
    }

    public Participant(String connectionId, String name)
    : this(connectionId)
    {
    }

    public string ConnectionId { get; set; }
    public string Name { get; set; } = String.Empty;
    public Guid Id { get; }
    public Guid ConnectionGuid { get; }
    public bool IsConnected { get; set; }

    internal ParticipantDto ToDTO() => new(Name, Id, IsConnected);


    public bool Equals(Participant? other) => other is not null && ConnectionId == other.ConnectionId;

    public override bool Equals(object? obj) => obj is not null 
            && (
                ReferenceEquals(this, obj)
                || obj is Participant other
                && Equals(other));

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Participant? left, Participant? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Participant? left, Participant? right)
    {
        return !Equals(left, right);
    }
}
