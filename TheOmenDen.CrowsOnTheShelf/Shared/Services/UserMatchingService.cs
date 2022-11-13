using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dao;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Services;
internal sealed class UserMatchingService
{
    private readonly ILogger<UserMatchingService> _logger;

    public UserMatchingService(ILogger<UserMatchingService> logger)
    {
        _logger= logger;
    }
}
