using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public interface IReminderScheduler
{
    Task<bool> RequestPermissionAsync(CancellationToken cancellationToken = default);

    Task ScheduleAsync(Reminder reminder, CancellationToken cancellationToken = default);

    Task CancelAsync(string reminderId, CancellationToken cancellationToken = default);
}
