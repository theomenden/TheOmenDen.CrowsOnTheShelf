namespace TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
public sealed class RoomStateDTO
{
    public RoomStateDTO()
    {
    }

    public RoomStateDTO(String roomName, List<ParticipantDto> participants, RoomSettings roomSettings)
    {
        RoomName = roomName;
        Participants = participants;
        RoomSettings = roomSettings;
    }
    public RoomStateDTO(String roomName, List<ParticipantDto> participants, RoomSettings roomSettings, bool isInprogress)
    {
        RoomName = roomName;
        Participants = participants;
        RoomSettings = roomSettings;
        IsGameInProgress = isInprogress;
    }

    public string RoomName { get; set; } = String.Empty;
    public List<ParticipantDto> Participants { get; set; } = new(10);
    public RoomSettings RoomSettings { get; set; } = new();
    public Boolean IsGameInProgress { get; set; }
}
