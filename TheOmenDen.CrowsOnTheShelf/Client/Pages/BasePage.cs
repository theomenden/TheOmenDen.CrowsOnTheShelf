using Microsoft.AspNetCore.Components;

namespace TheOmenDen.CrowsOnTheShelf.Client.Pages;

public abstract class BasePage: ComponentBase
{
    [Inject] protected NavigationManager NavigationManager { get; init; }
}
