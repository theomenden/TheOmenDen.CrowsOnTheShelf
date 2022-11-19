using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Pages;

public partial class Room: BasePage
{
    [Inject] private ISecretSantaEventService SecretSantaService { get; init; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (SecretSantaService.RoomState is null)
        {
            NavigationManager.NavigateTo("/");
        }
    } 
}
