using System.Timers;

namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

public sealed class TurnTimer: IDisposable
{
    public event EventHandler<Int32>? TurnTimerChanged;
    public Int32 RemainingTime { get; private set; } = 0;
    private System.Timers.Timer _timer;

    internal TurnTimer()
    {
        _timer = new();
        _timer.Interval= 1000;
        _timer.Elapsed += TimerElapsed;
    }

    internal void StartTimer(Int32 time)
    {
        RemainingTime= time;
        _timer.Start();
        TurnTimerChanged?.Invoke(this, RemainingTime);
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        RemainingTime -= 1;
        TurnTimerChanged?.Invoke(this, RemainingTime);
        
        if (RemainingTime <= 0)
        {
            _timer.Stop();
        }
    }

    internal void Stop() => _timer.Stop();
    
    internal void SetTime(Int32 timeRemaining) => RemainingTime= timeRemaining;

    public void Dispose()
    {
        _timer.Dispose();
        GC.SuppressFinalize(this);
    }
}
