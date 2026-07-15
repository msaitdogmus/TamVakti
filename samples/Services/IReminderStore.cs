using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public interface IReminderStore
{
    Task<IReadOnlyList<Reminder>> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(IEnumerable<Reminder> reminders, CancellationToken cancellationToken = default);
}
