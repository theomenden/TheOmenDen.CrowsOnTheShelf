using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using TheOmenDen.CrowsOnTheShelf.Models;

namespace TheOmenDen.CrowsOnTheShelf.Utils;

internal sealed class GraphClientAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public GraphClientAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, GraphOptions configuration) : base(provider, navigation)
    {
        ConfigureHandler(
                       authorizedUrls: [configuration.BaseAddress ?? String.Empty],
                                  scopes: configuration.Scopes);
    }
}