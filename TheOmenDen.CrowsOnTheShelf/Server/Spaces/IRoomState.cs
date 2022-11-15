using TheOmenDen.CrowsOnTheShelf.Server.Lobbies;

namespace TheOmenDen.CrowsOnTheShelf.Server.Spaces;

internal interface IRoomState
{
    Task EnterAsync(CancellationToken cancellationToken = default);
    Task AddParticipantAsync(Participant participant, bool isReconnection, CancellationToken cancellationToken = default);
    Task RemovePlayerAsync(Participant contact, CancellationToken cancellationToken = default);
}
