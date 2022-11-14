// The 'From' and 'To' fields are automatically populated with the values specified by the binding settings.
//
// You can also optionally configure the default From/To addresses globally via host.config, e.g.:
//
// {
//   "sendGrid": {
//      "to": "user@host.com",
//      "from": "Azure Functions <samples@functions.com>"
//   }
// }
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOntheShelf.Emails
{
    public class CrowsOnTheShelfFunction
    {
        [FunctionName("CrowsOnTheShelfFunction")]
        [return: SendGrid(ApiKey = "crowsforemails", To = "{CustomerEmail}", From = "support@theomenden")]
        public SendGridMessage Run([QueueTrigger("crowsontheline", Connection = "")]SecretSantaGame game, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed order: {game.Code}");

            var message = new SendGridMessage()
            {
                Subject = $"We're cawing on you to join our Secret Santa!"
            };

            message.AddContent(System.Net.Mime.MediaTypeNames.Text.Plain, $"{game.CustomerName}, your order ({game.OrderId}) is being processed!");
            return message;
        }
    }
    public class Order
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
}
