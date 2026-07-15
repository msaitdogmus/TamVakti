namespace TamVakti.Sample.Models;

public sealed class ReminderDraft
{
    public string Title { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.Today.AddDays(1);

    public TimeSpan Time { get; set; } = new(9, 0, 0);

    public ImportanceLevel Importance { get; set; } = ImportanceLevel.Normal;

    public RecurrenceKind Recurrence { get; set; }

    public DateTimeOffset ScheduledFor => new(
        Date.Date + Time,
        TimeZoneInfo.Local.GetUtcOffset(Date.Date + Time));

    public IReadOnlyDictionary<string, string> Validate(DateTimeOffset now)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(Title))
        {
            errors[nameof(Title)] = "Enter a title for the reminder.";
        }
        else if (Title.Trim().Length > 80)
        {
            errors[nameof(Title)] = "Keep the title under 80 characters.";
        }

        if (Note.Length > 500)
        {
            errors[nameof(Note)] = "Keep the note under 500 characters.";
        }

        if (ScheduledFor <= now.AddMinutes(1))
        {
            errors[nameof(Date)] = "Choose a time at least one minute from now.";
        }

        return errors;
    }

    public Reminder ToReminder() => new()
    {
        Title = Title.Trim(),
        Note = Note.Trim(),
        ScheduledFor = ScheduledFor,
        Importance = Importance,
        Recurrence = Recurrence
    };
}
