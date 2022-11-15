using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOmenDen.CrowsOnTheShelf.Shared.Models;
public sealed class RoomSettings
{
    public RoomSettings() {}

    public RoomSettings(String gameCode, String gameName, bool isPrivate)
    {
        GameCode= gameCode;
        GameName= gameName;
        IsPrivateRoom= isPrivate;
    }

    public RoomSettings(RoomSettings? roomSettings)
    {
        if (roomSettings is null)
        {
            return;
        }

        GameCode = roomSettings.GameCode;
        GameName = roomSettings.GameName;
        IsPrivateRoom = roomSettings.IsPrivateRoom;
    }

    public string GameCode { get; set; } = String.Empty;
    public string GameName { get; set; } = String.Empty;
    public Boolean IsPrivateRoom { get; set; } = true;
}
