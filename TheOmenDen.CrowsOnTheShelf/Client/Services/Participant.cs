using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

public sealed class Participant: IEquatable<Participant>
{
    private int _score;
    private bool _isGiftPurchased = false;
    private int? _position = null;
    public event EventHandler<bool> IsGiftPurchasedChanged;

    internal Participant(ParticipantDto participant)
    {

    }

    public Guid Id { get; set; }
    public String Name { get; set; }
    public Boolean IsConnected { get; set; }
    
    public Boolean IsGiftPurchased
    {
        get => _isGiftPurchased;
        set
        {
            if (value == _isGiftPurchased)
            {
                return;
            }
            _isGiftPurchased= value;
            IsGiftPurchasedChanged?.Invoke(this, _isGiftPurchased);
        }
    }

    public Int32? Position
    {
        get => _position;
        set
        {
            if (value == _position)
            {
                return;
            }
            _position = value;
            IsGiftPurchasedChanged?.Invoke(this, _isGiftPurchased);
        }
    }

    public bool Equals(Participant? other) =>
        other is not null
        && (ReferenceEquals(this, other)
            || Id.Equals(other.Id));

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) 
                                                || obj is Participant other 
                                                && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Participant? left, Participant? right) => Equals(left, right);

    public static bool operator !=(Participant? left, Participant? right) => !Equals(left, right);
}
