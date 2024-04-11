using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Graph;
using Microsoft.Kiota.Http.HttpClientLibrary;
using TheOmenDen.CrowsOnTheShelf.Utils;

namespace TheOmenDen.CrowsOnTheShelf.Services;

public class GraphClientFactory(IAccessTokenProviderAccessor accessor, HttpClient httpClient, ILogger<GraphClientFactory> logger)
{
    private GraphServiceClient? _graphServiceClient;

    public GraphServiceClient GetGraphServiceClient()
    {
        if (_graphServiceClient is not null)
        {
            return _graphServiceClient;
        }

        logger.LogInformation("Creating GraphServiceClient since none were discovered");

        var requestAdapter = new HttpClientRequestAdapter(
            new BlazorAuthProvider(accessor), null, null, httpClient);
        _graphServiceClient = new GraphServiceClient(requestAdapter);

        return _graphServiceClient;
    }
}