using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TheOmenDen.CrowsOnTheShelf.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class SendEmailController : ControllerBase
{
    private readonly ILogger<SendEmailController> _logger;

    public SendEmailController(ILogger<SendEmailController> logger)
    {
        _logger= logger;
    }


}
