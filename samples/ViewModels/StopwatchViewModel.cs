using System.Collections.ObjectModel;
using Microsoft.Maui.Dispatching;
using TamVakti.Sample.Models;

namespace TamVakti.Sample.ViewModels;

public sealed class StopwatchViewModel : IDisposable
{
    private readonly IDispatcherTimer refreshTimer;

    public StopwatchViewModel(IDispatcher dispatcher)
    {
        refreshTimer = dispatcher.CreateTimer();
        refreshTimer.Interval = TimeSpan.FromMilliseconds(100);
        refreshTimer.Tick += OnRefreshTimerTick;
        refreshTimer.Start();

        AddTimer();
    }

    public ObservableCollection<StopwatchTimer> Timers { get; } = [];

    public event EventHandler? ElapsedChanged;

    public StopwatchTimer AddTimer()
    {
        var timer = new StopwatchTimer($"Timer {Timers.Count + 1}");
        Timers.Add(timer);
        return timer;
    }

    public void RemoveTimer(StopwatchTimer timer)
    {
        if (Timers.Count > 1)
        {
            Timers.Remove(timer);
        }
    }

    public void StartAll()
    {
        foreach (var timer in Timers)
        {
            timer.Start();
        }
    }

    public void PauseAll()
    {
        foreach (var timer in Timers)
        {
            timer.Pause();
        }
    }

    public void ResetAll()
    {
        foreach (var timer in Timers)
        {
            timer.Reset();
        }

        ElapsedChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        refreshTimer.Stop();
        refreshTimer.Tick -= OnRefreshTimerTick;
    }

    private void OnRefreshTimerTick(object? sender, EventArgs e)
    {
        if (Timers.Any(x => x.IsRunning))
        {
            ElapsedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
