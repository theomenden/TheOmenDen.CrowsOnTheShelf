using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class ChatMessageDisplay: ComponentBase
{
    [Parameter] public ChatMessage Message { get; set; } = null!;
}
