using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;

namespace TheOmenDen.CrowsOnTheShelf.Client.Pages;

public partial class Lobby : BasePage, IDisposable
{
    [Inject] private ISecretSantaEventService SecretSantaService { get; init; }

    private string? _selectedInProgressRoom;
    private string? _selectedAwaitingRoom;
    private RoomStateDTO? _selectedRoom = null;
    private string _roomCode = String.Empty;
    private bool _isWaitingToJoin = false;
    private bool _hasJoinFailed = false;

    private readonly List<RoomStateDTO> _roomsInProgress = new(10);
    private readonly List<RoomStateDTO> _roomsAwaiting = new(10);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (SecretSantaService.ParticipantGuid == null)
        {
            NavigationManager.NavigateTo("/");
        }

        SecretSantaService.RoomListChanged += OnRoomListChanged;
        SecretSantaService.RoomSettingsChanged += OnRoomSettingsChanged;
    }

    private async Task JoinRoom(string roomName, string roomCode)
    {
        _isWaitingToJoin = true;
        await InvokeAsync(StateHasChanged);
        _hasJoinFailed = !(await SecretSantaService.JoinRoom(roomName, roomCode));
        _isWaitingToJoin = false;
        await InvokeAsync(StateHasChanged);
    }

    private void SelectRoom(RoomStateDTO room)
    {
        _selectedRoom = room;
    }

    private void OnRoomListChanged(object? sender, EventArgs e) => StateHasChanged();
    private void OnRoomSettingsChanged(object? sender, EventArgs e) => StateHasChanged();



    public void Dispose()
    {
        SecretSantaService.RoomListChanged -= OnRoomListChanged;
        SecretSantaService.RoomSettingsChanged -= OnRoomSettingsChanged;
        GC.SuppressFinalize(this);
    }
}
