using Microsoft.AspNetCore.SignalR;

namespace TheOmenDen.CrowsOnTheShelf.Server.Hubs;

public class ClientLogHub: Hub
{
    public const string HubUrl = "/log";

    private ILogger<ClientLogHub> _logger;

    public ClientLogHub(ILogger<ClientLogHub> logger)
    {
        _logger= logger;
    }

    public void Fatal(string type, string stackTrace, string message) =>
        _logger.LogError("Client unhandled exception: {Message}\n{Type}\n{StackTrace}", message, type, stackTrace);
}
