using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models;

public sealed record InviteCodeDto(InviteCodeId Id, string Code);