using TheOmenDen.Shared.Enumerations;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Enumerations;

public sealed record ChatMessageType(String Name, Int32 Id) : EnumerationBase<ChatMessageType>(Name, Id)
{
    public static readonly ChatMessageType EventFlow = new (nameof(EventFlow), 1);
    public static readonly ChatMessageType Chat= new(nameof(Chat), 2);
}
