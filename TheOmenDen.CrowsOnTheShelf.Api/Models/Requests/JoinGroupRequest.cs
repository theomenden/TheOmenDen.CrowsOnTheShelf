using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models.Requests;

public sealed record JoinGroupRequest(string InviteCode, string Email, GiftGroupId GroupId, string ConnectionId);