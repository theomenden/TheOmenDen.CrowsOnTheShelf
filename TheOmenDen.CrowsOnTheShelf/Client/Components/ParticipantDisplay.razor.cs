using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class ParticipantDisplay: ComponentBase, IDisposable
{
    [Parameter] public bool ShowGiftPurchased { get; set; }

    [Parameter] public Participant Participant { get; set; } = null!;

    protected override void OnInitialized()
    {
        Participant.IsGiftPurchasedChanged += OnGiftPurchased;
    }

    public void OnGiftPurchased(object? sender, bool isGiftPurchased) => StateHasChanged();

    public void Dispose()
    {
        Participant.IsGiftPurchasedChanged -= OnGiftPurchased;
    }
}
