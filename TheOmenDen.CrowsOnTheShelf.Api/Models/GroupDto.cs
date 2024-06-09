using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models;

public sealed record GroupDto(GiftGroupId Id, string Name, InviteCodeDto InviteCode);