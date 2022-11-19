using TheOmenDen.CrowsOnTheShelf.Shared.Models.Enumerations;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;

public sealed record ChatMessage(ChatMessageType MessageType, String? ParticipantName, String? Message);
