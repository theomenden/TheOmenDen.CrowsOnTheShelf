using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Services;
internal sealed class SecretSantaService
{
    private readonly ILogger<SecretSantaService> _logger;

    public SecretSantaService(ILogger<SecretSantaService> logger)
    {
        _logger= logger;
    }

    public Task AddSecretSantaGame(SecretSantaGame secretSantaGame)
    {
        var santaStorageModel = new SecretSantaStorageModel
        {
            Budget = secretSantaGame.Budget,
            Code = secretSantaGame.Code,
            ParticipantEmails = secretSantaGame.ParticipantEmails,
            OccurringAt = secretSantaGame.OccurringAt
        };

        return Task.CompletedTask;
    }

    public bool CheckIfUsersHaveAccepted(SecretSantaStorageModel secretSantaGame) =>
        secretSantaGame.HaveAllParticipantsAccepted();
}
