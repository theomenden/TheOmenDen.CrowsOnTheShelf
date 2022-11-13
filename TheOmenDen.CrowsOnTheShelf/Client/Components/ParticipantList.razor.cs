using Microsoft.AspNetCore.Components;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class ParticipantList: ComponentBase
{
    private readonly List<String> _participants = new(10);

    protected override void OnInitialized()
    {
        BuildTestParticipants();
    }

    private void BuildTestParticipants()
    {
        for (var i = 0; i < 11; i++)
        {
            _participants.Add($"test{i}@test.com");
        }
    }
}
