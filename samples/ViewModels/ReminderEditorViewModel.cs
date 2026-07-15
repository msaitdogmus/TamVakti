using System.ComponentModel;
using System.Runtime.CompilerServices;
using TamVakti.Sample.Models;
using TamVakti.Sample.Services;

namespace TamVakti.Sample.ViewModels;

public sealed class ReminderEditorViewModel : INotifyPropertyChanged
{
    private readonly ReminderService reminderService;
    private string title = string.Empty;
    private string note = string.Empty;
    private DateTime date = DateTime.Today.AddDays(1);
    private TimeSpan time = new(9, 0, 0);
    private ImportanceLevel importance = ImportanceLevel.Normal;
    private RecurrenceKind recurrence;
    private string? titleError;
    private string? dateError;
    private string? message;
    private bool isSaving;

    public ReminderEditorViewModel(ReminderService reminderService)
    {
        this.reminderService = reminderService;
    }

    public IReadOnlyList<ImportanceLevel> ImportanceOptions { get; } =
        Enum.GetValues<ImportanceLevel>();

    public IReadOnlyList<RecurrenceKind> RecurrenceOptions { get; } =
        Enum.GetValues<RecurrenceKind>();

    public string Title
    {
        get => title;
        set
        {
            if (SetField(ref title, value))
            {
                TitleError = null;
            }
        }
    }

    public string Note
    {
        get => note;
        set => SetField(ref note, value);
    }

    public DateTime Date
    {
        get => date;
        set
        {
            if (SetField(ref date, value))
            {
                DateError = null;
            }
        }
    }

    public TimeSpan Time
    {
        get => time;
        set
        {
            if (SetField(ref time, value))
            {
                DateError = null;
            }
        }
    }

    public ImportanceLevel Importance
    {
        get => importance;
        set => SetField(ref importance, value);
    }

    public RecurrenceKind Recurrence
    {
        get => recurrence;
        set => SetField(ref recurrence, value);
    }

    public string? TitleError
    {
        get => titleError;
        private set => SetField(ref titleError, value);
    }

    public string? DateError
    {
        get => dateError;
        private set => SetField(ref dateError, value);
    }

    public string? Message
    {
        get => message;
        private set => SetField(ref message, value);
    }

    public bool IsSaving
    {
        get => isSaving;
        private set
        {
            if (SetField(ref isSaving, value))
            {
                OnPropertyChanged(nameof(CanSave));
            }
        }
    }

    public bool CanSave => !IsSaving;

    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        if (IsSaving)
        {
            return false;
        }

        ClearErrors();
        IsSaving = true;

        try
        {
            var reminder = await reminderService.CreateAsync(
                BuildDraft(),
                cancellationToken);

            Message = $"Reminder set for {reminder.ScheduledFor:g}.";
            return true;
        }
        catch (ReminderValidationException exception)
        {
            ApplyErrors(exception.Errors);
            Message = "Check the highlighted fields.";
            return false;
        }
        catch (OperationCanceledException)
        {
            Message = "Saving was cancelled.";
            return false;
        }
        catch (Exception)
        {
            Message = "The reminder could not be saved. Try again.";
            return false;
        }
        finally
        {
            IsSaving = false;
        }
    }

    public void SetImportance(ImportanceLevel level)
    {
        Importance = level;
    }

    private ReminderDraft BuildDraft() => new()
    {
        Title = Title,
        Note = Note,
        Date = Date,
        Time = Time,
        Importance = Importance,
        Recurrence = Recurrence
    };

    private void ApplyErrors(IReadOnlyDictionary<string, string> errors)
    {
        errors.TryGetValue(nameof(ReminderDraft.Title), out var titleMessage);
        errors.TryGetValue(nameof(ReminderDraft.Date), out var dateMessage);

        TitleError = titleMessage;
        DateError = dateMessage;
    }

    private void ClearErrors()
    {
        TitleError = null;
        DateError = null;
        Message = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
