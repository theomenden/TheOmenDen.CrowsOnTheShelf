using System.Security.Cryptography;
using FastEndpoints;
using TheOmenDen.CrowsOnTheShelf.Api.Models;

namespace TheOmenDen.CrowsOnTheShelf.Api.Services;

public interface IInviteCodeGenerationService
{
    InviteCode GenerateInviteCode(IEnumerable<string> authorizedEmails);
}

[RegisterService<IInviteCodeGenerationService>(LifeTime.Singleton)]
internal sealed class InviteCodeGenerationService : IInviteCodeGenerationService
{
    private readonly Dictionary<DayOfWeek, string> _prefixes = new(
        [new(DayOfWeek.Monday, "Crows"),
        new(DayOfWeek.Tuesday, "Corvids"),
        new(DayOfWeek.Wednesday, "Ravens"),
        new(DayOfWeek.Thursday, "Omens"),
        new(DayOfWeek.Friday, "Eldritch"),
        new(DayOfWeek.Saturday, "Crows"),
        new(DayOfWeek.Sunday, "Corvids")]
    );
    private const string AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public InviteCode GenerateInviteCode(IEnumerable<string> authorizedEmails)
    {
        var prefix = _prefixes[DateTime.UtcNow.DayOfWeek];
        var codeSuffix = GenerateRandomAlphanumericString(8);
        return new InviteCode
        {
            Code = $"{prefix}-{codeSuffix}",
            AuthorizedEmails = authorizedEmails.ToArray(),
            ExpiresAt = DateTime.UtcNow.AddDays(7), // Set expiry date to 7 days from now
            UsesLeft = authorizedEmails.Count() // Uses left equals the number of authorized emails
        };
    }

    private string GenerateRandomAlphanumericString(int length)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var chars = new char[length];
        for (var i = 0; i < length; i++)
        {
            if (randomBytes[i] % AllowedChars.Length >= 0 &&
                randomBytes[i] % AllowedChars.Length < AllowedChars.Length)
            {
                chars[i] = AllowedChars[randomBytes[i] % AllowedChars.Length];
            }
        }
        return new(chars);
    }
}