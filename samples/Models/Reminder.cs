namespace TamVakti.Sample.Models;

public sealed class Reminder
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");

    public string Title { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public DateTimeOffset ScheduledFor { get; set; } = DateTimeOffset.Now.AddHours(1);

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;

    public ImportanceLevel Importance { get; set; } = ImportanceLevel.Normal;

    public RecurrenceKind Recurrence { get; set; }

    public ReminderStatus Status { get; set; } = ReminderStatus.Scheduled;
}

public enum ImportanceLevel
{
    Low,
    Normal,
    High
}

public enum RecurrenceKind
{
    None,
    Daily,
    Weekly,
    Monthly
}

public enum ReminderStatus
{
    Scheduled,
    Archived
}
