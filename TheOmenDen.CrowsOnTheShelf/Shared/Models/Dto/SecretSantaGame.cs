namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
public sealed record SecretSantaGame(String Code, DateTime OccurringAt, Decimal Budget, IEnumerable<String> ParticipantEmails);