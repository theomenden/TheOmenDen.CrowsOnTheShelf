using TheOmenDen.CrowsOnTheShelf.Api.Models;
using TheOmenDen.CrowsOnTheShelf.Api.Models.Requests;
using TheOmenDen.CrowsOnTheShelf.Api.Sagas;

namespace TheOmenDen.CrowsOnTheShelf.Api.Endpoints;

public sealed class CreateGroupEndpoint : Endpoint<CreateGroupRequest, GroupDto>
{
    public ICreateGroupSaga CreateGroupSaga { get; set; }

    public override void Configure()
    {
        Post("groups");
        AllowFormData(true);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var result = await CreateGroupSaga.ExecuteSagaAsync(request, cancellationToken);

        await SendAsync(result, cancellation: cancellationToken);
    }
}