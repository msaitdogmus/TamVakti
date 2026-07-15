using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public interface IStopwatchService
{
    Task<IReadOnlyList<StopwatchTimer>> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(IEnumerable<StopwatchTimer> timers, CancellationToken cancellationToken = default);

    void UpdateNotification(IReadOnlyList<StopwatchTimer> timers);

    void StopNotification();
}
