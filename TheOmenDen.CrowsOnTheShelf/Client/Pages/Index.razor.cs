using Blazorise;
using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Components;

namespace TheOmenDen.CrowsOnTheShelf.Client.Pages;

public partial class Index: ComponentBase
{
    [Inject] private IModalService ModalService { get; init; }

    private Task LaunchCreateSantaGroupAsync() => ModalService.Show<CreateSantaGroup>(String.Empty, new ModalInstanceOptions
    {
        Scrollable = true,
        Size = ModalSize.ExtraLarge,
        Border = Border.Is2.Light.OnAll.Rounded,
        Centered = true,
        UseModalStructure = false
    });
}
