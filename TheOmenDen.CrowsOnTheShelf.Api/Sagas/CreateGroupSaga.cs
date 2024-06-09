using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using TheOmenDen.CrowsOnTheShelf.Api.Contexts;
using TheOmenDen.CrowsOnTheShelf.Api.Models;
using TheOmenDen.CrowsOnTheShelf.Api.Models.Requests;
using TheOmenDen.CrowsOnTheShelf.Api.Services;

namespace TheOmenDen.CrowsOnTheShelf.Api.Sagas;

public interface ICreateGroupSaga
{
    Task<GroupDto> ExecuteSagaAsync(CreateGroupRequest request, CancellationToken cancellationToken);
}

[RegisterService<ICreateGroupSaga>(LifeTime.Scoped)]
internal sealed class CreateGroupSaga(ILogger<CreateGroupSaga> logger, IGiftGroupService giftGroupService, ServiceBusClient client)
: ICreateGroupSaga
{
    public async Task<GroupDto> ExecuteSagaAsync(CreateGroupRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating group {GroupName}", request.GroupName);
        var group = await giftGroupService.CreateGroupAsync(request.GroupName, request.AuthorizedEmails, cancellationToken);

        logger.LogInformation("Creating Service Bus message from group");
        await using var sender = client.CreateSender("cawing-mail-topic");
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(group)));

        logger.LogInformation("Sending group to mail topic");
        await sender.SendMessageAsync(message, cancellationToken);

        return group;
    }
}
