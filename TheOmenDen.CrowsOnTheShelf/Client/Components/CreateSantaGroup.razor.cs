using Blazorise;
using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Client.Services;
using TheOmenDen.CrowsOnTheShelf.Shared.Models;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
using TheOmenDen.CrowsOnTheShelf.Shared.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class CreateSantaGroup: ComponentBase
{
    [Inject] private IModalService ModalService { get; init; }

    [Inject] private ISecretSantaEventService SantaEventService { get; init; }

    private RoomSettings _roomSettings = new();

    private string _roomName = String.Empty;

    private string _gameCode = GameCodeGenerator.GenerateGameCode();

    private readonly List<String> _participantEmails = new(20);

    private decimal? _budget = 25m;

    private DateTime? _occurringAt = DateTime.UtcNow;

    private string _participantEmail;

    private Task CloseAsync() => ModalService.Hide();

    private Task AddParticipantAsync()
    {
        if (String.IsNullOrWhiteSpace(_participantEmail))
        {
            return Task.CompletedTask;
        }

        _participantEmails.Add(_participantEmail);

        return InvokeAsync(StateHasChanged);
    }

    private Task OnClearAsync()
    {
        _budget = 25m;
        _participantEmails.Clear();
        _participantEmail = String.Empty;
        _occurringAt = DateTime.UtcNow;
        _roomName= String.Empty;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task OnSubmitAsync()
    {
        var santaGame = new SecretSantaGame(_occurringAt.GetValueOrDefault(DateTime.UtcNow.AddDays(7)), _budget.GetValueOrDefault(25m), _participantEmails);
        
        _roomSettings.SecretSantaGame = santaGame;
        _roomSettings.IsPrivateRoom = true;
        _roomSettings.GameCode= _gameCode;
        _roomSettings.GameName = _roomName;

        if (await SantaEventService.CreateRoom(_roomName, _roomSettings))
        {
            await SantaEventService.JoinRoom(_roomSettings.GameName, _roomSettings.GameCode);
        }
    }
}
