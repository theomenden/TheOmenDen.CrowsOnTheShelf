using System.Security.Claims;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;
using TheOmenDen.CrowsOnTheShelf.Extensions;

namespace TheOmenDen.CrowsOnTheShelf.Services;

public sealed class GraphUserAccountFactory(IAccessTokenProviderAccessor accessor, ILogger<GraphUserAccountFactory> logger, GraphClientFactory clientFactory) : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor)
{
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        var initialUser = await base.CreateUserAsync(account, options);

        if (!(initialUser?.Identity?.IsAuthenticated ?? false))
        {
            return initialUser ??
                   throw new InvalidOperationException("The authentication state does not contain a valid user.");
        }

        try
        {
            await AddGraphInfoToClaimsAsync(initialUser);
        }
        catch (AccessTokenNotAvailableException ex)
        {
            logger.LogError(ex, "Failed to get access token for Graph API");
        }
        catch (ServiceException ex)
        {
            logger.LogError(ex, "Failed to get user info from Graph API");
        }
        return initialUser ?? throw new InvalidOperationException("The authentication state does not contain a valid user.");
    }

    private async Task AddGraphInfoToClaimsAsync(ClaimsPrincipal claimsPrincipal)
    {
        var graphClient = clientFactory.GetGraphServiceClient();
        var user = await graphClient.Me.GetAsync(options =>
            options.QueryParameters.Select = ["displayName", "mail", "mailboxSettings", "userPrincipalName"])
                   ?? throw new InvalidOperationException("Could not retrieve user from Microsoft Graph");

        claimsPrincipal.AddUserGraphInfo(user);

        try
        {
            var photo = await graphClient.Me.Photos["48x48"].Content.GetAsync();
            claimsPrincipal.AddUserGraphPhoto(photo);
        }
        catch (ODataError dataError)
        {
            logger.LogError(dataError, "Failed to get user photo from Graph API: {Code}", dataError?.Error?.Code);

            if (dataError?.Error?.Code != "ImageNotFound")
            {
                throw dataError ?? new Exception("Failed to get user photo from Graph API");
            }
        }
    }
}