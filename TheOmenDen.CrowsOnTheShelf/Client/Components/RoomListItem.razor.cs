using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class RoomListItem: ComponentBase
{
    [Parameter] public RoomStateDTO Room { get; set; } = null!;
}
