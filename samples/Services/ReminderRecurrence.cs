using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public static class ReminderRecurrence
{
    public static DateTimeOffset? GetNextOccurrence(
        Reminder reminder,
        DateTimeOffset completedAt)
    {
        if (reminder.Recurrence == RecurrenceKind.None)
        {
            return null;
        }

        var next = reminder.ScheduledFor;
        do
        {
            next = reminder.Recurrence switch
            {
                RecurrenceKind.Daily => next.AddDays(1),
                RecurrenceKind.Weekly => next.AddDays(7),
                RecurrenceKind.Monthly => AddMonthWithoutDrift(next),
                _ => throw new ArgumentOutOfRangeException(nameof(reminder))
            };
        }
        while (next <= completedAt);

        return next;
    }

    private static DateTimeOffset AddMonthWithoutDrift(DateTimeOffset value)
    {
        var firstOfNextMonth = new DateTimeOffset(
            value.Year,
            value.Month,
            1,
            value.Hour,
            value.Minute,
            value.Second,
            value.Offset).AddMonths(1);

        var day = Math.Min(
            value.Day,
            DateTime.DaysInMonth(firstOfNextMonth.Year, firstOfNextMonth.Month));

        return new DateTimeOffset(
            firstOfNextMonth.Year,
            firstOfNextMonth.Month,
            day,
            value.Hour,
            value.Minute,
            value.Second,
            value.Offset);
    }
}
