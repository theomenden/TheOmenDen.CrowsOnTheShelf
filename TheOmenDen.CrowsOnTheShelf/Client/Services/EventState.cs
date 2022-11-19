using TheOmenDen.CrowsOnTheShelf.Shared.Models;

namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

internal sealed class EventState
{
    public event EventHandler? RoundStarted;
    public event EventHandler? ActivePlayerStarted;
    public event EventHandler<ChatMessage>? ChatMessageReceived;

    private List<ChatMessage> _chatLog = new(100);

    public IEnumerable<ChatMessage> ChatLog => _chatLog;

    public Int32 CurrentRound { get; private set; } = 0;
    public Int32 RoundCount { get; private set; } = 0;
    
    public TurnTimer TurnTimer { get; } = new();

    internal EventState()
    {
    }

    internal void NewRoundStart(int currentRound, int roundCount, ChatMessage chatMessage)
    {
        CurrentRound = currentRound;
        RoundCount = roundCount;
        RoundStarted?.Invoke(this, EventArgs.Empty);

        if (chatMessage is not null)
        {
            AddChatMessage(chatMessage);
        }
    }

    #region Chat Messages
    internal void AddChatMessage(ChatMessage cm)
    {
        if (_chatLog.Count >= 50)
        {
            _chatLog.RemoveRange(0, _chatLog.Count - 49);
        }
        _chatLog.Add(cm);
        ChatMessageReceived?.Invoke(this, cm);
    }
    #endregion
}
