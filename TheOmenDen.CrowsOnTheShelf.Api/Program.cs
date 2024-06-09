using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Azure.Identity;
using FastEndpoints.Swagger;
using Microsoft.Extensions.Azure;
using TheOmenDen.CrowsOnTheShelf.Api;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
            .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph"))
            .AddInMemoryTokenCaches();
builder.Services.AddAuthorization();

builder.Services.RegisterServicesFromTheOmenDenCrowsOnTheShelfApi();

builder.Services.AddFastEndpoints(o =>
    {
        o.SourceGeneratorDiscoveredTypes.AddRange(TheOmenDen.CrowsOnTheShelf.Api.DiscoveredTypes.All);
    })
    .AddAntiforgery()
    .AddIdempotency()
    .AddResponseCaching()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.Title = "Crows on the Shelf API";
            s.Description = "API for Crows on the Shelf";
            s.Version = "v1";
        };
        o.RemoveEmptyRequestSchema = true;
        o.SerializerSettings = s =>
        {
            s.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            s.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        };
        o.ShortSchemaNames = true;
    });

builder.Services.AddAzureClients(configureClient =>
{
    configureClient.AddServiceBusClientWithNamespace(builder.Configuration.GetConnectionString("AzureServiceBus"));
    configureClient.UseCredential(new DefaultAzureCredential());
});

using var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication()
    .UseAuthorization()
    .UseAntiforgery()
    .UseResponseCaching()
    .UseOutputCache()
    .UseDefaultExceptionHandler()
    .UseFastEndpoints(o =>
    {
        o.Endpoints.RoutePrefix = "api";
        o.Endpoints.ShortNames = true;
    })
    .UseSwaggerGen();
app.Run();
