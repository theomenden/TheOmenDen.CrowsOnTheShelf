using TheOmenDen.CrowsOnTheShelf.Api.Models;
using TheOmenDen.CrowsOnTheShelf.Api.Models.Requests;
using TheOmenDen.CrowsOnTheShelf.Api.Sagas;

namespace TheOmenDen.CrowsOnTheShelf.Api.Endpoints;

public sealed class JoinGroupEndpoint : Endpoint<JoinGroupRequest, bool>
{
    public IJoinGroupSaga JoinGroupSaga { get; set; }

    public override void Configure()
    {
        Post("groups/{groupId}/join");
        AllowFormData(true);
        AllowAnonymous();
    }

    public override async Task HandleAsync(JoinGroupRequest req, CancellationToken ct)
    {
        var result = await JoinGroupSaga.ExecuteSagaAsync(req, ct);

        if (!result)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        await SendNoContentAsync(ct);
    }
}