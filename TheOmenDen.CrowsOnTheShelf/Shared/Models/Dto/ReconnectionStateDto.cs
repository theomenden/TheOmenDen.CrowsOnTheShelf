namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
public sealed class ReconnectionStateDto
{
    public ReconnectionStateDto(Guid participantId, RoomStateDTO? roomState)
    {
        ParticipantId= participantId;
        RoomState = roomState;
    }

    public Guid ParticipantId { get; set; }
    public RoomStateDTO? RoomState { get; set; }
}
