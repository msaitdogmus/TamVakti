# Code samples

This folder is a buildable, Android-only .NET MAUI sample based on the architecture of Tam Vakti. It demonstrates the parts that are useful in a portfolio review while keeping the production application and its private configuration out of the repository.

## Start here

For the clearest end-to-end example, read the reminder files in this order:

1. `Models/ReminderDraft.cs` — input normalization and field validation
2. `ViewModels/ReminderEditorViewModel.cs` — form state, save state and user-facing errors
3. `Services/ReminderService.cs` — permission, scheduling and storage coordination
4. `Services/JsonReminderStore.cs` — serialized local persistence
5. `Platforms/Android/AndroidReminderScheduler.cs` — native alarm setup
6. `Platforms/Android/ReminderReceiver.cs` — notification delivery
7. `Views/ReminderEditorPage.xaml` — the MAUI form bound to the ViewModel

## Directory guide

| Folder | Purpose |
| --- | --- |
| `Models` | Reminder, habit, stopwatch and weather data types |
| `Services` | Persistence, recurrence, weather, themes and application coordination |
| `ViewModels` | UI state and actions without Android-specific code |
| `Views` | Selected XAML controls and the reminder editor page |
| `Platforms/Android` | `AlarmManager`, `BroadcastReceiver` and notification-channel integration |
| `Converters` | Small presentation helpers shared by XAML views |

## Design notes

- Android APIs stay behind `IReminderScheduler`, keeping the ViewModels easy to follow and test.
- Local JSON writes use a temporary file so an interrupted save does not replace valid state with a partial document.
- Reminder creation compensates for a failed disk write by cancelling the alarm that was just scheduled.
- Recurring reminders are advanced until their next date is in the future. Monthly reminders preserve the requested day when the target month allows it.
- Stopwatch elapsed time is calculated from timestamps; the UI timer only asks the view to refresh and does not act as the time source.

## Build

From this directory:

```bash
dotnet build TamVakti.Sample.csproj -c Release
```

The samples do not include signing material, API credentials, store packages, the complete localization catalog or the full production UI. Code in this folder is licensed under the MIT License.
