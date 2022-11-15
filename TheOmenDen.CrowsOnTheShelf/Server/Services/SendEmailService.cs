using SendGrid;
using SendGrid.Helpers.Mail;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Server.Services;

public sealed class SendEmailService
{
    private const string supportEmailAddress = "support@theomenden.com";
    private const string supportTeamName = "Your friendly crows";
    private readonly ISendGridClient _sendGridClient;
    private ILogger<SendEmailService> _logger;

    public SendEmailService(ISendGridClient sendGridClient, ILogger<SendEmailService> logger)
    {
        _logger= logger;
        _sendGridClient= sendGridClient;
    }



    public async Task<bool> SendEmailAsync(Contact contact)
    {
        var msg = new SendGridMessage();
        var fromAddress = new EmailAddress(supportEmailAddress, supportTeamName );
        var recipients = new List<EmailAddress>();

        msg.SetSubject("You've been invited to a gift exchange!");
        msg.SetFrom(fromAddress);
        msg.AddTos(recipients);
        msg.PlainTextContent = contact.Message;

        var response = await _sendGridClient.SendEmailAsync (msg);

        return response.IsSuccessStatusCode;
    }
}
