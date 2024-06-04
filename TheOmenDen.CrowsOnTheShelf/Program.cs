using BlazorDexie.Extensions;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Captcha.ReCaptcha;
using Blazorise.Icons.FontAwesome;
using Blazorise.LoadingIndicator;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using TheOmenDen.CrowsOnTheShelf;

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

    builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithSensitiveDataMasking(options =>
        {
            options.Mode = MaskingMode.Globally;
            options.MaskingOperators = [new CreditCardMaskingOperator(), new EmailAddressMaskingOperator()];
        })
        .WriteTo.BrowserConsole()
        .CreateLogger(), dispose: true));

    builder.Services.AddBlazorise(options =>
        {
            options.Immediate = true;
            options.ProductToken = builder.Configuration.GetSection("Blazorise")["Token"];
            options.IconStyle = IconStyle.DuoTone;
        })
        .AddBootstrap5Providers()
        .AddFontAwesomeIcons()
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