using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TamVakti.Sample.Models;
using TamVakti.Sample.Services;

namespace TamVakti.Sample.ViewModels;

public sealed class ReminderListViewModel : INotifyPropertyChanged
{
    private readonly ReminderService reminderService;
    private bool isBusy;

    public ReminderListViewModel(ReminderService reminderService)
    {
        this.reminderService = reminderService;
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
            var items = await reminderService.GetScheduledAsync(cancellationToken);

            Reminders.Clear();
            foreach (var reminder in items)
            {
                Reminders.Add(reminder);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task AddAsync(
        ReminderDraft draft,
        CancellationToken cancellationToken = default)
    {
        var reminder = await reminderService.CreateAsync(draft, cancellationToken);
        InsertByDate(reminder);
    }

    public async Task CompleteAsync(
        Reminder reminder,
        CancellationToken cancellationToken = default)
    {
        Reminders.Remove(reminder);

        var updated = await reminderService.CompleteAsync(
            reminder.Id,
            DateTimeOffset.Now,
            cancellationToken);

        if (updated?.Status == ReminderStatus.Scheduled)
        {
            InsertByDate(updated);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void InsertByDate(Reminder reminder)
    {
        var index = 0;
        while (index < Reminders.Count &&
               Reminders[index].ScheduledFor <= reminder.ScheduledFor)
        {
            index++;
        }

        Reminders.Insert(index, reminder);
    }

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
