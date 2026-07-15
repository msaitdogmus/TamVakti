namespace TamVakti.Sample.Models;

public sealed class StopwatchTimer
{
    private DateTimeOffset? startedAt;
    private TimeSpan accumulated;

    public StopwatchTimer(string name)
    {
        Name = name;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; }

    public bool IsRunning => startedAt.HasValue;

    public List<Lap> Laps { get; } = [];

    public TimeSpan Elapsed => startedAt is null
        ? accumulated
        : accumulated + (DateTimeOffset.UtcNow - startedAt.Value);

    public void Start()
    {
        if (!IsRunning)
        {
            startedAt = DateTimeOffset.UtcNow;
        }
    }

    public void Pause()
    {
        if (startedAt is null)
        {
            return;
        }

        accumulated = Elapsed;
        startedAt = null;
    }

    public Lap AddLap()
    {
        var total = Elapsed;
        var previous = Laps.Count == 0 ? TimeSpan.Zero : Laps[^1].Total;
        var lap = new Lap(Laps.Count + 1, total - previous, total);

        Laps.Add(lap);
        return lap;
    }

    public void Reset()
    {
        startedAt = null;
        accumulated = TimeSpan.Zero;
        Laps.Clear();
    }
}

public sealed record Lap(int Number, TimeSpan Split, TimeSpan Total);
