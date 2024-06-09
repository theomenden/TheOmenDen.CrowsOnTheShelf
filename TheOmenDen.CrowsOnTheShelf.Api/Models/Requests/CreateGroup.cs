using Mediator;

namespace TheOmenDen.CrowsOnTheShelf.Api.Models.Requests;

public sealed record CreateGroupRequest(string GroupName, DateTime StartDate, decimal Budget, string CreatorEmail, IEnumerable<string> AuthorizedEmails);