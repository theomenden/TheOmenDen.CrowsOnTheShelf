using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;

public sealed record ParticipantGift(ParticipantDto Participant, bool HasPurchasedGift);
