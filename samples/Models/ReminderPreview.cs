namespace TamVakti.Showcase.Models;

public sealed record ReminderPreview(
    string Id,
    string Title,
    DateTimeOffset DueAt,
    ReminderPriority Priority)
{
    public bool IsDue(DateTimeOffset now) => DueAt <= now;
}

public enum ReminderPriority
{
    Low,
    Medium,
    High
}
