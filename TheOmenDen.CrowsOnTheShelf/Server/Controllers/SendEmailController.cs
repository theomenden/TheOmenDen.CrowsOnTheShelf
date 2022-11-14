using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TheOmenDen.CrowsOnTheShelf.Server.Controllers;
[ApiController]
[Route("[controller]")]
public class SendEmailController : ControllerBase
{
    private readonly ILogger<SendEmailController> _logger;
    private readonly ISendEmailService _sendEmailService;

    public SendEmailController(ILogger<SendEmailController> logger, ISendEmailService _sendEmailService)
    {
        _logger= logger;
        _sendEmailService= _sendEmailService;
    }


}
