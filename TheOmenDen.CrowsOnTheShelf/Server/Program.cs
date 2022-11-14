using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog.Events;
using Serilog;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Builder;
using TheOmenDen.CrowsOnTheShelf.Server.Hubs;
using SendGrid.Extensions.DependencyInjection;

#region Bootstrap Logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithProcessName()
    .Enrich.WithEnvironmentUserName()
    .Enrich.WithMemoryUsage()
    .WriteTo.Async(a =>
    {
        a.File("./logs/log-.txt", rollingInterval: RollingInterval.Day);
        a.Console();
        a.Debug();
    })
    .CreateBootstrapLogger();
#endregion

try
{
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
        .AddInMemoryTokenCaches();

    builder.Services.AddSendGrid(options => options.ApiKey = builder.Configuration["crowsagainstemails"]);
    builder.Services.AddSignalR();
    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { System.Net.Mime.MediaTypeNames.Application.Octet });
    });

    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    await using var app = builder.Build();

    app.UseResponseCompression();
    app.UseResponseCaching();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseWebAssemblyDebugging();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();


    app.MapRazorPages();
    app.MapControllers();

    app.MapHub<ChatHub>("/chatHub");
    app.MapFallbackToFile("index.html");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal("An error occurred before {AppName} could launch: {@Ex}", nameof(TheOmenDen.CrowsOnTheShelf), ex);
}
finally
{
    Log.CloseAndFlush();
}