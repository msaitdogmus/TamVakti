using Android.App;
using Android.Content;
using AndroidX.Core.App;

namespace TamVakti.Sample.Platforms.Android;

[BroadcastReceiver(Enabled = true, Exported = false)]
public sealed class ReminderReceiver : BroadcastReceiver
{
    public const string IdKey = "reminder_id";
    public const string TitleKey = "reminder_title";
    public const string NoteKey = "reminder_note";

    private const string ChannelId = "tamvakti_reminders";

    public override void OnReceive(Context? context, Intent? intent)
    {
        if (context is null || intent is null)
        {
            return;
        }

        var id = intent.GetStringExtra(IdKey) ?? Guid.NewGuid().ToString("N");
        var title = intent.GetStringExtra(TitleKey) ?? "Tam Vakti";
        var note = intent.GetStringExtra(NoteKey) ?? string.Empty;

        EnsureChannel(context);

        var launchIntent = context.PackageManager?
            .GetLaunchIntentForPackage(context.PackageName ?? string.Empty);

        var contentIntentFlags = PendingIntentFlags.UpdateCurrent;
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            contentIntentFlags |= PendingIntentFlags.Immutable;
        }

        var contentIntent = launchIntent is null
            ? null
            : PendingIntent.GetActivity(
                context,
                0,
                launchIntent,
                contentIntentFlags);

        var style = new NotificationCompat.BigTextStyle();
        style.BigText(note);

        var builder = new NotificationCompat.Builder(context, ChannelId);
        builder.SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo);
        builder.SetColor(global::Android.Graphics.Color.Rgb(22, 199, 132));
        builder.SetContentTitle(title);
        builder.SetContentText(note);
        builder.SetStyle(style);
        builder.SetAutoCancel(true);
        builder.SetPriority(NotificationCompat.PriorityHigh);

        if (contentIntent is not null)
        {
            builder.SetContentIntent(contentIntent);
        }

        var notification = builder.Build();

        NotificationManagerCompat.From(context)?
            .Notify(AndroidReminderScheduler.GetRequestCode(id), notification);
    }

    private static void EnsureChannel(Context context)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return;
        }

        var manager = context.GetSystemService(Context.NotificationService) as NotificationManager;
        if (manager?.GetNotificationChannel(ChannelId) is not null)
        {
            return;
        }

        var channel = new NotificationChannel(
            ChannelId,
            "Reminders",
            NotificationImportance.High)
        {
            Description = "Scheduled reminder notifications"
        };

        channel.EnableVibration(true);
        manager?.CreateNotificationChannel(channel);
    }
}
