using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class EventTimerComponent: ComponentBase, IDisposable
{
    [Inject] private ISecretSantaEventService SecretSantaEventService { get; init; }

    private int TimeString => SecretSantaEventService.EventState.TurnTimer.RemainingTime;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        SecretSantaEventService.EventState.TurnTimer.TurnTimerChanged += TurnTimerChanged;
    }
    
    private void TurnTimerChanged(object? sender, int time)
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        SecretSantaEventService.EventState.TurnTimer.TurnTimerChanged -= TurnTimerChanged;
        GC.SuppressFinalize(this);
    }
}
