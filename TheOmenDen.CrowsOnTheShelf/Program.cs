using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheOmenDen.CrowsOnTheShelf;
using Azure.Identity;
using BlazorDexie.Extensions;
using Blazorise;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;
using Blazorise.LoadingIndicator;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Sensitive;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using TheOmenDen.CrowsOnTheShelf.Services;
using TheOmenDen.CrowsOnTheShelf.Utils;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessName()
    .Enrich.WithThreadName()
    .Enrich.WithMemoryUsage()
    .Enrich.WithSensitiveDataMasking(options => options.MaskingOperators = [new CreditCardMaskingOperator(), new EmailAddressMaskingOperator()])
    .WriteTo.Async(a =>
    {
        a.Console(theme: AnsiConsoleTheme.Code);
        a.Debug(new CompactJsonFormatter());
        a.BrowserConsole();
    })
    .CreateLogger();

try
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);

    builder.Services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders().AddSerilog(Log.Logger, dispose: true));

    builder.Services.AddBlazorise(options =>
        {
            options.Immediate = true;
            options.ProductToken = builder.Configuration.GetSection("Blazorise")["Token"];
        })
        .AddFluentUI2Providers()
        .AddFluentUIIcons()
        .AddLoadingIndicator()
        .AddBlazoriseGoogleReCaptcha(options =>
        {
            options.Theme = ReCaptchaTheme.Dark;
            options.Size = ReCaptchaSize.Invisible;
            options.SiteKey = builder.Configuration.GetSection("ReCaptcha")["SiteKey"];
        });

    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    builder.Services.AddMicrosoftGraphClient(builder.Configuration);
    builder.Services.AddDexieWrapper();

    await builder.Build().RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred while running the application: {Message}", ex.Message);
}
finally
{
    await Log.CloseAndFlushAsync();
}