namespace TamVakti.Sample.Models;

public sealed class Habit
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Title { get; set; } = string.Empty;

    public HabitFrequency Frequency { get; set; } = HabitFrequency.Daily;

    public TimeSpan PreferredTime { get; set; } = new(9, 0, 0);

    public string? ReminderId { get; set; }

    public bool IsActive { get; set; } = true;
}

public enum HabitFrequency
{
    Daily,
    Weekly,
    Monthly
}
