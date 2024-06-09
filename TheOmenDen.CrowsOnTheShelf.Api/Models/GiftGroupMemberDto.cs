using TheOmenDen.CrowsOnTheShelf.Api.Models.EntityIds;
using TheOmenDen.CrowsOnTheShelf.Api.States;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models;

public sealed record GiftGroupMemberDto(GroupMemberId Id, string Email, MemberState State, GroupDto Group);