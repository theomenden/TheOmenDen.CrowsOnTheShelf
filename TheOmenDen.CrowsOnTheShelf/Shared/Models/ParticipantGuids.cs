using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;
public sealed class ParticipantGuids
{
    public ParticipantGuids(Guid playerId, Guid connectionId)
    {
        PlayerId = playerId;
        ConnectionId = connectionId;
    }

    public Guid PlayerId { get; set; }

    public Guid ConnectionId { get; set; }
}
