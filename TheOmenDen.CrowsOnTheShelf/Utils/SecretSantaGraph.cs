using Microsoft.Graph.Models;
using TheOmenDen.CrowsOnTheShelf.Models;

namespace TheOmenDen.CrowsOnTheShelf.Utils;

public class SecretSantaGraph
{
    private readonly List<EventParticipant> participants = [];

    // Assuming you've fetched the participants from Microsoft Graph
    public void AddParticipants(IEnumerable<User> users)
    {
        foreach (var user in users)
        {
            participants.Add(new EventParticipant(user.Id, user.DisplayName));
        }
    }

    public IReadOnlyDictionary<EventParticipant, EventParticipant> GeneratePairs()
    {
        var rng = new Random();
        var shuffledParticipants = participants.OrderBy(a => rng.Next()).ToList();

        Dictionary<EventParticipant, EventParticipant> pairs = [];
        for (var i = 0; i < shuffledParticipants.Count; i++)
        {
            var giver = shuffledParticipants[i];
            var receiver = shuffledParticipants[(i + 1) % shuffledParticipants.Count]; // Creates a cycle
            pairs[giver] = receiver;
        }

        return pairs; // Dictionary implicitly implements IReadOnlyDictionary
    }

}