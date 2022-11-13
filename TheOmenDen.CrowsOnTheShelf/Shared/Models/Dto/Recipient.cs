namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

public sealed record Recipient(Guid Id, String Email, IEnumerable<UserGift> UserGifts);
