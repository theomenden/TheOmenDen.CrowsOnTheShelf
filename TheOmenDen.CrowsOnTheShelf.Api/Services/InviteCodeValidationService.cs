using System.Data;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TheOmenDen.CrowsOnTheShelf.Api.Contexts;
using TheOmenDen.CrowsOnTheShelf.Api.Models;

namespace TheOmenDen.CrowsOnTheShelf.Api.Services;

public interface IInviteCodeValidationService
{
    ValueTask<bool> ValidateInviteCodeAsync(string code, CancellationToken cancellationToken = default);
    ValueTask<bool> ValidateInviteCodeAsync(string code, string email, CancellationToken cancellationToken = default);
}

[RegisterService<IInviteCodeValidationService>(LifeTime.Scoped)]
internal sealed class InviteCodeValidationService(IDbContextFactory<CrowsOnTheShelfContext> contextFactory, ILogger<InviteCodeValidationService> logger)
    : IInviteCodeValidationService
{
    public async ValueTask<bool> ValidateInviteCodeAsync(string code, string email, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrWhiteSpace(code) || String.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            var inviteCode =
                await context.InviteCodes
                    .Include(ic => ic.AuthorizedEmails)
                    .FirstOrDefaultAsync(ic => ic.Code == code, cancellationToken: cancellationToken);

            if (InviteIsCodeValid(inviteCode) || EmailIsAuthorized(inviteCode, email))
            {
                return false;
            }

            inviteCode!.UsesLeft--;
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update invite code: {Ex}", ex.Message);
            return false;
        }
        catch (DBConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to update invite code due to a concurrency issue: {Ex}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Class}{Method}: {Ex}", nameof(InviteCodeValidationService), nameof(ValidateInviteCodeAsync), ex.Message);
            throw;
        }
    }

    private static bool EmailIsAuthorized(InviteCode? inviteCode, string email) => inviteCode is not null
        && inviteCode.AuthorizedEmails.Contains(email);

    private static bool InviteIsCodeValid(InviteCode? inviteCode) => inviteCode is not null
                                                                     && inviteCode.ExpiresAt > DateTime.UtcNow
                                                                     && inviteCode.UsesLeft > 0;
    public async ValueTask<bool> ValidateInviteCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        try
        {
            var inviteCode =
                await context.InviteCodes.FirstOrDefaultAsync(ic => ic.Code == code,
                    cancellationToken: cancellationToken);
            if (inviteCode is null || inviteCode.ExpiresAt <= DateTime.UtcNow || inviteCode.UsesLeft <= 0)
            {
                return false;
            }

            inviteCode.UsesLeft--;
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Failed to update invite code: {Ex}", ex.Message);
            return false;
        }
        catch (DBConcurrencyException ex)
        {
            logger.LogError(ex, "Failed to update invite code due to a concurrency issue: {Ex}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Class}{Method}: {Ex}", nameof(InviteCodeValidationService), nameof(ValidateInviteCodeAsync), ex.Message);
            throw;
        }
    }
}