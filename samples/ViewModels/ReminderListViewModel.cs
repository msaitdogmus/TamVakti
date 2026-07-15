using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TamVakti.Sample.Models;
using TamVakti.Sample.Services;

namespace TamVakti.Sample.ViewModels;

public sealed class ReminderListViewModel : INotifyPropertyChanged
{
    private readonly IReminderStore store;
    private readonly IReminderScheduler scheduler;
    private List<Reminder> allReminders = [];
    private bool isBusy;

    public ReminderListViewModel(IReminderStore store, IReminderScheduler scheduler)
    {
        this.store = store;
        this.scheduler = scheduler;
    }

    public ObservableCollection<Reminder> Reminders { get; } = [];

    public bool IsBusy
    {
        get => isBusy;
        private set => SetField(ref isBusy, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        try
        {
            var items = await store.LoadAsync(cancellationToken);
            allReminders = items.ToList();

            Reminders.Clear();
            foreach (var reminder in allReminders
                         .Where(x => x.Status == ReminderStatus.Scheduled)
                         .OrderBy(x => x.ScheduledFor))
            {
                Reminders.Add(reminder);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task AddAsync(Reminder reminder, CancellationToken cancellationToken = default)
    {
        allReminders.Add(reminder);
        Reminders.Add(reminder);
        await store.SaveAsync(allReminders, cancellationToken);
        await scheduler.ScheduleAsync(reminder, cancellationToken);
    }

    public async Task ArchiveAsync(Reminder reminder, CancellationToken cancellationToken = default)
    {
        reminder.Status = ReminderStatus.Archived;
        Reminders.Remove(reminder);

        await scheduler.CancelAsync(reminder.Id, cancellationToken);
        await store.SaveAsync(allReminders, cancellationToken);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
