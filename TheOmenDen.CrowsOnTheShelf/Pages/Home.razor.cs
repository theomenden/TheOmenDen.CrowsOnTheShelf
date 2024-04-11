using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace TheOmenDen.CrowsOnTheShelf.Pages;

public partial class Home : ComponentBase
{
    [CascadingParameter] Task<AuthenticationState> AuthenticationStateTask { get; set; }
    [Inject] private Services.GraphClientFactory clientFactory { get; init; }

    private Microsoft.Graph.Models.User? _user;
    private Microsoft.Graph.Models.Organization? _organization;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateTask;

        if (authState is null)
        {
            await base.OnInitializedAsync();
            return;
        }

        using var client = clientFactory.GetGraphServiceClient();

        _user = await client.Me.GetAsync();

        if (_user is not null)
        {
            _organization = (await client.Organization.GetAsync()).Value?.FirstOrDefault();
        }
    }
}