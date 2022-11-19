using Blazored.LocalStorage;
using TheOmenDen.CrowsOnTheShelf.Client.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecretSantaServices(this IServiceCollection services)
    {
        services.AddSingleton<ISecretSantaEventService, SecretSantaEventService>();
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddBlazoredLocalStorage();
        return services;
    }
}
