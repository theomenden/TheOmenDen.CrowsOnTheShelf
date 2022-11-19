using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;
public sealed class RoomSettings
{
    public RoomSettings() {}

    public RoomSettings(String gameCode, String gameName, Int32 rounds, bool isPrivate, SecretSantaGame secretSantaGame)
    {
        GameCode= gameCode;
        GameName= gameName;
        Rounds= rounds;
        IsPrivateRoom= isPrivate;
        SecretSantaGame = secretSantaGame;
    }

    public RoomSettings(RoomSettings? roomSettings)
    {
        if (roomSettings is null)
        {
            return;
        }

        GameCode = roomSettings.GameCode;
        GameName = roomSettings.GameName;
        Rounds = roomSettings.Rounds;
        IsPrivateRoom = roomSettings.IsPrivateRoom;
        SecretSantaGame = roomSettings.SecretSantaGame;
    }

    public string GameCode { get; set; } = String.Empty;
    public string GameName { get; set; } = String.Empty;
    public Boolean IsPrivateRoom { get; set; } = true;
    public Int32 Rounds { get; set; } = 4;
    public SecretSantaGame SecretSantaGame { get; set; }
}
