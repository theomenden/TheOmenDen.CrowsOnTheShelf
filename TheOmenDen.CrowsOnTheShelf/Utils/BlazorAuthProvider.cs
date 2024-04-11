using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace TheOmenDen.CrowsOnTheShelf.Utils;

public sealed class BlazorAuthProvider(IAccessTokenProviderAccessor accessor) : IAuthenticationProvider
{
    public async Task AuthenticateRequestAsync(RequestInformation request,
                                               Dictionary<string, object>? additionalAuthenticationContext = null,
                                               CancellationToken cancellationToken = default)
    {
        var result = await accessor.TokenProvider.RequestAccessToken();

        if (result.TryGetToken(out var token))
        {
            request.Headers.Add("Authorization", $"Bearer {token.Value}");
        }
    }
}