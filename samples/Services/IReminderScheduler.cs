using TamVakti.Showcase.Models;

namespace TamVakti.Showcase.Services;

public interface IReminderScheduler
{
    Task ScheduleAsync(
        ReminderPreview reminder,
        CancellationToken cancellationToken = default);

    Task CancelAsync(
        string reminderId,
        CancellationToken cancellationToken = default);
}
