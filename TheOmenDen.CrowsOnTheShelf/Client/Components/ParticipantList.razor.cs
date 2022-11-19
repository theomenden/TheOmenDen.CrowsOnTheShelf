using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class ParticipantList: ComponentBase, IDisposable
{
    [Parameter] public Boolean ShowGiftPurchased { get; set; } = true;
    [Inject]private ISecretSantaEventService SecretSantaEventService { get; init; }

    private readonly List<Participant> _participants = new(10);

    protected override async Task OnInitializedAsync()
    {
        _participants.AddRange(SecretSantaEventService.Participants);

        await base.OnInitializedAsync();
        SecretSantaEventService.ParticipantListChanged += OnParticipantListChanged;
    }

    private void OnParticipantListChanged(object? sender, EventArgs e) => StateHasChanged();

    public void Dispose()
    {
        SecretSantaEventService.ParticipantListChanged -= OnParticipantListChanged;
        GC.SuppressFinalize(this);
    }
}
