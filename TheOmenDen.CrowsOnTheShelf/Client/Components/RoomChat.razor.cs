using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class RoomChat : IDisposable
{
    [Parameter] public bool IsInChatMode { get; set; } = false;

    [Inject] private ISecretSantaEventService SantaEventService { get; init; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        SantaEventService.EventState.ChatMessageReceived += ChatMessageReceived;
    }
    
    private void ChatMessageReceived(object? sender, CrowsOnTheShelf.Shared.Models.ChatMessage cm) => StateHasChanged();

    public void Dispose() => SantaEventService.EventState.ChatMessageReceived -= ChatMessageReceived;
}
