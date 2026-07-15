using Android.App;
using Android.Content;
using Android.OS;
using Microsoft.Maui.ApplicationModel;
using TamVakti.Sample.Models;
using TamVakti.Sample.Services;

namespace TamVakti.Sample.Platforms.Android;

public sealed class AndroidReminderScheduler : IReminderScheduler
{
    private readonly Context context;

    public AndroidReminderScheduler()
    {
        context = global::Android.App.Application.Context;
    }

    public async Task<bool> RequestPermissionAsync(
        CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return true;
        }

        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        }

        return status == PermissionStatus.Granted;
    }

    public Task ScheduleAsync(
        Reminder reminder,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (reminder.ScheduledFor <= DateTimeOffset.Now)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reminder),
                "A reminder must be scheduled in the future.");
        }

        var intent = new Intent(context, typeof(ReminderReceiver))
            .PutExtra(ReminderReceiver.IdKey, reminder.Id)
            .PutExtra(ReminderReceiver.TitleKey, reminder.Title)
            .PutExtra(ReminderReceiver.NoteKey, reminder.Note);

        var pendingIntentFlags = PendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            pendingIntentFlags |= PendingIntentFlags.Immutable;
        }

        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            GetRequestCode(reminder.Id),
            intent,
            pendingIntentFlags)
            ?? throw new InvalidOperationException("Could not create the alarm intent.");

        var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager
            ?? throw new InvalidOperationException("Alarm service is unavailable.");

        var triggerAt = reminder.ScheduledFor.ToUnixTimeMilliseconds();

        if (OperatingSystem.IsAndroidVersionAtLeast(31) &&
            !alarmManager.CanScheduleExactAlarms())
        {
            alarmManager.SetAndAllowWhileIdle(
                AlarmType.RtcWakeup,
                triggerAt,
                pendingIntent);
        }
        else if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            alarmManager.SetExactAndAllowWhileIdle(
                AlarmType.RtcWakeup,
                triggerAt,
                pendingIntent);
        }
        else
        {
            alarmManager.SetExact(
                AlarmType.RtcWakeup,
                triggerAt,
                pendingIntent);
        }

        return Task.CompletedTask;
    }

    public Task CancelAsync(
        string reminderId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var intent = new Intent(context, typeof(ReminderReceiver));
        var pendingIntentFlags = PendingIntentFlags.NoCreate;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            pendingIntentFlags |= PendingIntentFlags.Immutable;
        }

        var pendingIntent = PendingIntent.GetBroadcast(
            context,
            GetRequestCode(reminderId),
            intent,
            pendingIntentFlags);

        if (pendingIntent is not null)
        {
            var alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            alarmManager?.Cancel(pendingIntent);
            pendingIntent.Cancel();
        }

        return Task.CompletedTask;
    }

    internal static int GetRequestCode(string value)
    {
        unchecked
        {
            var hash = 17;
            foreach (var character in value)
            {
                hash = hash * 31 + character;
            }

            return hash & int.MaxValue;
        }
    }
}
