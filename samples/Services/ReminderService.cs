using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public sealed class ReminderService
{
    private readonly IReminderStore store;
    private readonly IReminderScheduler scheduler;
    private readonly SemaphoreSlim gate = new(1, 1);

    public ReminderService(IReminderStore store, IReminderScheduler scheduler)
    {
        this.store = store;
        this.scheduler = scheduler;
    }

    public async Task<IReadOnlyList<Reminder>> GetScheduledAsync(
        CancellationToken cancellationToken = default)
    {
        var reminders = await store.LoadAsync(cancellationToken);

        return reminders
            .Where(item => item.Status == ReminderStatus.Scheduled)
            .OrderBy(item => item.ScheduledFor)
            .ToList();
    }

    public async Task<Reminder> CreateAsync(
        ReminderDraft draft,
        CancellationToken cancellationToken = default)
    {
        var errors = draft.Validate(DateTimeOffset.Now);
        if (errors.Count > 0)
        {
            throw new ReminderValidationException(errors);
        }

        if (!await scheduler.RequestPermissionAsync(cancellationToken))
        {
            throw new InvalidOperationException(
                "Notification permission is required to create a reminder.");
        }

        var reminder = draft.ToReminder();

        await gate.WaitAsync(cancellationToken);
        try
        {
            await scheduler.ScheduleAsync(reminder, cancellationToken);

            try
            {
                var items = (await store.LoadAsync(cancellationToken)).ToList();
                items.Add(reminder);
                await store.SaveAsync(items, cancellationToken);
            }
            catch
            {
                await scheduler.CancelAsync(reminder.Id, CancellationToken.None);
                throw;
            }

            return reminder;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<Reminder?> CompleteAsync(
        string reminderId,
        DateTimeOffset completedAt,
        CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var items = (await store.LoadAsync(cancellationToken)).ToList();
            var reminder = items.FirstOrDefault(item => item.Id == reminderId);
            if (reminder is null)
            {
                return null;
            }

            await scheduler.CancelAsync(reminder.Id, cancellationToken);

            var nextOccurrence = ReminderRecurrence.GetNextOccurrence(
                reminder,
                completedAt);

            if (nextOccurrence is null)
            {
                reminder.Status = ReminderStatus.Archived;
            }
            else
            {
                reminder.ScheduledFor = nextOccurrence.Value;
                await scheduler.ScheduleAsync(reminder, cancellationToken);
            }

            await store.SaveAsync(items, cancellationToken);
            return reminder;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task SnoozeAsync(
        string reminderId,
        TimeSpan delay,
        CancellationToken cancellationToken = default)
    {
        if (delay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay));
        }

        await gate.WaitAsync(cancellationToken);
        try
        {
            var items = (await store.LoadAsync(cancellationToken)).ToList();
            var reminder = items.FirstOrDefault(item => item.Id == reminderId)
                ?? throw new KeyNotFoundException("The reminder no longer exists.");

            reminder.ScheduledFor = DateTimeOffset.Now.Add(delay);
            reminder.Status = ReminderStatus.Scheduled;

            await scheduler.ScheduleAsync(reminder, cancellationToken);
            await store.SaveAsync(items, cancellationToken);
        }
        finally
        {
            gate.Release();
        }
    }
}

public sealed class ReminderValidationException : Exception
{
    public ReminderValidationException(IReadOnlyDictionary<string, string> errors)
        : base("The reminder contains invalid values.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
}
