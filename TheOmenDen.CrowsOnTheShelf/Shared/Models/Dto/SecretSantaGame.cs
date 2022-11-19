namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
public sealed record SecretSantaGame(DateTime OccurringAt, Decimal Budget, IEnumerable<String> ParticipantEmails);