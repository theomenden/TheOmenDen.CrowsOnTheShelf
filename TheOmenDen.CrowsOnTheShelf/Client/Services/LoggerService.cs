using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

public sealed class LoggerService : ILoggerService
{
    private readonly HubConnection _hubConnection;

    public LoggerService(NavigationManager navigationManager)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(navigationManager.ToAbsoluteUri("/log"))
            .Build();
        _ = _hubConnection.StartAsync();
    }

    public Task Fatal(Exception exception)
    => _hubConnection.InvokeAsync("Fatal", exception.GetType().ToString(), exception.StackTrace, exception.Message);
}
