using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public sealed class NoOpReminderScheduler : IReminderScheduler
{
    public Task<bool> RequestPermissionAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(true);

    public Task ScheduleAsync(Reminder reminder, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task CancelAsync(string reminderId, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
