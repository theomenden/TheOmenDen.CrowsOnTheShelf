using Blazorise;
using Microsoft.AspNetCore.Components;
using TheOmenDen.CrowsOnTheShelf.Shared.Models.Dto;
using TheOmenDen.CrowsOnTheShelf.Shared.Services;

namespace TheOmenDen.CrowsOnTheShelf.Client.Components;

public partial class CreateSantaGroup: ComponentBase
{
    [Inject] private IModalService ModalService { get; init; }

    private readonly List<String> _participantEmails = new(20);

    private decimal? _budget = 0m;

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

    private Task OnSubmitAsync()
    {
        var gameCode = GameCodeGenerator.GenerateGameCode();

        var santaGame = new SecretSantaGame(gameCode, _occurringAt.GetValueOrDefault(DateTime.UtcNow.AddDays(7)), _budget.GetValueOrDefault(25m), _participantEmails);
        
        return Task.CompletedTask;
    }
}
