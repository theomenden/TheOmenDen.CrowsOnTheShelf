using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace TheOmenDen.CrowsOnTheShelf.Pages;

public partial class Home : ComponentBase
{
    [Inject] private IAccessTokenProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
}