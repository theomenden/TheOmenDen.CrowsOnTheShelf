using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog;
using TheOmenDen.CrowsOnTheShelf.Services;
using TheOmenDen.CrowsOnTheShelf.Utils;

/// <summary>
/// Adds services and implements methods to use Microsoft Graph SDK.
/// </summary>
internal static class GraphClientExtensions
{
    /// <summary>
    /// Extension method for adding the Microsoft Graph SDK to <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">Provided service collection <see cref="IServiceCollection"/></param>
    /// <param name="configuration">The MS Graph configuration to build requests <see cref="IConfiguration"/></param>
    /// <returns>The supplied <see cref="IServiceCollection"/> for further chaining</returns>
    public static IServiceCollection AddMicrosoftGraphClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(_ => new HttpClient
        { BaseAddress = new Uri(configuration.GetSection("MicrosoftGraph")["BaseUrl"] ?? String.Empty) });
        services.AddTransient<GraphClientAuthorizationMessageHandler>();

        services.AddMsalAuthentication<RemoteAuthenticationState, RemoteUserAccount>(options =>
            {
                var scopes = configuration.GetSection("MicrosoftGraph")["Scopes"]?.Split(' ') ?? Array.Empty<string>();

                if (scopes.Length == 0)
                {
                    Log.Warning("No scopes were provided for Microsoft Graph, this may cause issues with the application");
                    Log.Information("Adding in default scope, User.Read");
                    scopes = ["User.Read"];
                }

                foreach (var scope in scopes)
                {
                    Log.Information("Adding scope {Scope} to the Microsoft Graph authentication provider", scope);
                    options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
                }

                configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            })
            .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, GraphUserAccountFactory>();

        services.AddScoped<GraphClientFactory>();
        return services;
    }
}
