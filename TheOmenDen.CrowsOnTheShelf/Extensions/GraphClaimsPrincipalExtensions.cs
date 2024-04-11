using System.Security.Claims;
using Microsoft.Graph.Models;
using TheOmenDen.CrowsOnTheShelf.Utils;

namespace TheOmenDen.CrowsOnTheShelf.Extensions;

public static class GraphClaimsPrincipalExtensions
{
    public static string GetUserGraphDateFormat(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.FindFirst(GraphClaimTypes.DateFormat)?.Value ?? "yyyy-MM-dd";

    public static string GetUserGraphEmail(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public static string GetUserGraphId(this ClaimsPrincipal claimsPrincipal) =>
    claimsPrincipal.FindFirst(GraphClaimTypes.Id)?.Value ?? string.Empty;

    public static string GetUserGraphPhoto(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.FindFirst(GraphClaimTypes.Photo)?.Value ?? string.Empty;

    public static string GetUserGraphTimeZone(this ClaimsPrincipal claimsPrincipal) =>
    claimsPrincipal.FindFirst(GraphClaimTypes.TimeZone)?.Value ?? string.Empty;

    public static string GetUserGraphUserPrincipalName(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.FindFirst(GraphClaimTypes.UserPrincipalName)?.Value ?? string.Empty;

    public static string GetUserGraphDisplayName(this ClaimsPrincipal claimsPrincipal) =>
    claimsPrincipal.FindFirst(GraphClaimTypes.DisplayName)?.Value ?? string.Empty;

    public static string GetUserGraphTimeFormat(this ClaimsPrincipal claimsPrincipal) =>
        claimsPrincipal.FindFirst(GraphClaimTypes.TimeFormat)?.Value ?? "HH:mm";

    public static void AddUserGraphInfo(this ClaimsPrincipal claimsPrincipal, User user)
    {
        if (claimsPrincipal.Identity is not ClaimsIdentity identity)
        {
            throw new InvalidOperationException("ClaimsPrincipal must have an identity to add user info");
        }

        identity.AddClaim(new Claim(GraphClaimTypes.DateFormat, user.MailboxSettings?.DateFormat ?? "yyyy-MM-dd"));
        identity.AddClaim(new Claim(GraphClaimTypes.Email, user.Mail ?? user.UserPrincipalName ?? String.Empty));
        identity.AddClaim(new Claim(GraphClaimTypes.TimeZone, user.MailboxSettings?.TimeZone ?? "UTC"));
        identity.AddClaim(new Claim(GraphClaimTypes.TimeFormat, user.MailboxSettings?.TimeFormat ?? "HH:mm"));
    }

    public static void AddUserGraphPhoto(this ClaimsPrincipal claimsPrincipal, Stream? photoStream)
    {
        if (claimsPrincipal.Identity is not ClaimsIdentity identity)
        {
            throw new InvalidOperationException("ClaimsPrincipal must have an identity to add user photo");
        }

        if (photoStream is null || photoStream == Stream.Null)
        {
            return;
        }

        using var memoryStream = new MemoryStream();
        photoStream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();
        var base64 = Convert.ToBase64String(bytes);
        var photo = $"data:image/png;base64,{base64}";

        identity.AddClaim(new Claim(GraphClaimTypes.Photo, photo));
    }
}