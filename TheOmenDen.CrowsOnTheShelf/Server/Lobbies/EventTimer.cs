using System.Timers;

namespace TheOmenDen.CrowsOnTheShelf.Server.Lobbies;

public sealed class EventTimer: IDisposable
{
    private System.Timers.Timer _timer;
    private DateTime _startTime;
    private DateTime _nextTrigger;

    public EventTimer(double interval, ElapsedEventHandler handler)
    {
        _timer = new(interval)
        {
            AutoReset = false
        };

        _timer.Elapsed+= handler;
    }

    public double TimeElapsed => (DateTime.UtcNow - _startTime).TotalMilliseconds;
    public double TimeRemaining => (_nextTrigger - DateTime.UtcNow).TotalMilliseconds;

    public double Change(double multiplier)
    {
        _timer.Stop();
        var newInterval = multiplier * TimeRemaining;

        _timer.Interval = newInterval;

        Start();

        return TimeRemaining;
    }

    public double Reset(double interval)
    {
        _timer.Stop();
        _timer.Interval = interval;
        Start();
        return TimeRemaining;
    }

    public void Start()
    {
        _startTime= DateTime.UtcNow;
        _nextTrigger= _startTime.Add(TimeSpan.FromMilliseconds(_timer.Interval));
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
