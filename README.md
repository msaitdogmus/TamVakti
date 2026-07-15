<div align="center">
  <img src="assets/branding/app-icon.png" alt="Tam Vakti app icon" width="112" />
  <h1>Tam Vakti</h1>
  <p><strong>Plan your time. Never miss the moment.</strong></p>
  <p>A focused productivity app that brings reminders, habit tracking, weather and multiple stopwatches together.</p>

  <p>
    <a href="https://play.google.com/store/apps/details?id=com.msds.tamvakti"><img alt="Get it on Google Play" src="https://img.shields.io/badge/Google_Play-Download-414141?style=for-the-badge&logo=googleplay&logoColor=white"></a>
  </p>

  <p>
    <img alt=".NET 10" src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white">
    <img alt=".NET MAUI for Android" src="https://img.shields.io/badge/.NET_MAUI-Android-512BD4">
    <img alt="C#" src="https://img.shields.io/badge/C%23-Modern-239120?logo=csharp&logoColor=white">
    <img alt="Android" src="https://img.shields.io/badge/Android-5.0%2B-3DDC84?logo=android&logoColor=white">
    <img alt="MIT License" src="https://img.shields.io/badge/sample_code-MIT-green">
  </p>
</div>

![Tam Vakti feature graphic](assets/branding/feature-graphic.jpg)

## About the app

Tam Vakti is designed for people who want a simple way to organize daily plans, important moments and focus sessions. Reminders can be created with a date, time, note and priority. The home screen keeps the next events visible without turning the app into a crowded task manager.

The current Android release is available on [Google Play](https://play.google.com/store/apps/details?id=com.msds.tamvakti).

### Highlights

- Date- and time-based reminders with three priority levels
- Native Android alarms with snooze support
- Voice input for quickly entering reminder titles
- Habit tracking with daily, weekly and monthly schedules
- Archive for completed reminders
- Multiple stopwatches running at the same time
- Foreground service support for background stopwatch sessions
- Location-based weather with manual location selection
- Configurable notification and alarm sounds
- Light and dark themes
- Localization for Afrikaans, Arabic, German, Greek, English, Spanish, French, Hindi, Japanese, Portuguese, Russian, Turkish and Chinese

## Screenshots

The latest release uses a separate reminder editor so the home screen stays clean and easy to scan.

<p align="center">
  <img src="assets/screenshots/home.png" alt="Tam Vakti home screen" width="260" />
  <img src="assets/screenshots/create-reminder.png" alt="Create reminder screen" width="260" />
  <img src="assets/screenshots/archive.png" alt="Reminder archive" width="260" />
</p>

<p align="center">
  <img src="assets/screenshots/habits.png" alt="Habit tracking" width="260" />
  <img src="assets/screenshots/stopwatch.png" alt="Multiple stopwatch screen" width="260" />
  <img src="assets/screenshots/notification-settings.png" alt="Notification and alarm settings" width="260" />
</p>

## Technology stack

| Area | Technology |
| --- | --- |
| Application framework | .NET 10 and .NET MAUI for Android |
| Language | C# |
| User interface | XAML, MAUI Controls and custom styles |
| Platform | Android 5.0+ (API 21+) |
| Distribution | Google Play via Android App Bundle |
| Application structure | Dependency injection, service abstractions and Android-specific implementations |
| Android integration | AlarmManager, BroadcastReceiver, Foreground Service and SpeechRecognizer |
| Device APIs | Geolocation, Preferences and local notifications |
| Networking | HttpClient-based weather and location requests |
| Persistence | Local JSON state and device preferences |
| Localization | Culture-aware resources for 13 languages |
| Release build | R8 shrinking and Android App Bundle |

## Architecture

The interface and application services are built with .NET MAUI for Android. Native features such as alarms, speech recognition and background stopwatch notifications are exposed through small interfaces and implemented in the Android platform layer.

```text
Android MAUI pages and controls
        │
View models and application services
        │
Models, local storage and HTTP services
        │
Platform contracts
        │
Android alarms, receivers, speech and foreground services
```

## Code samples

The [`samples`](samples) directory contains a small, buildable Android project that mirrors the main layers of Tam Vakti. It is more than a collection of disconnected snippets: the reminder editor, validation, local storage and Android alarm implementation form one readable flow.

```text
samples/
|-- AppBootstrap.cs
|-- TamVakti.Sample.csproj
|-- Converters/
|   `-- ImportanceColorConverter.cs
|-- Models/
|   |-- Habit.cs
|   |-- Reminder.cs
|   |-- ReminderDraft.cs
|   |-- StopwatchTimer.cs
|   `-- WeatherSnapshot.cs
|-- Platforms/Android/
|   |-- AndroidManifest.xml
|   |-- AndroidReminderScheduler.cs
|   `-- ReminderReceiver.cs
|-- Services/
|   |-- IReminderScheduler.cs
|   |-- IReminderStore.cs
|   |-- JsonReminderStore.cs
|   |-- ReminderRecurrence.cs
|   |-- ReminderService.cs
|   |-- ThemeManager.cs
|   `-- WeatherClient.cs
|-- ViewModels/
|   |-- ReminderEditorViewModel.cs
|   |-- ReminderListViewModel.cs
|   `-- StopwatchViewModel.cs
`-- Views/
    |-- ReminderCard.xaml
    |-- ReminderEditorPage.xaml
    `-- ReminderEditorPage.xaml.cs
```

### Reminder creation flow

The reminder example follows the same separation used throughout the app:

1. `ReminderEditorPage` contains the Android-friendly MAUI form and binds user input to `ReminderEditorViewModel`.
2. `ReminderDraft` combines the selected date and time, normalizes the input and returns field-level validation errors.
3. `ReminderService` asks for notification permission, schedules the Android alarm and then persists the reminder. If saving fails, it cancels the alarm so storage and Android do not drift apart.
4. `AndroidReminderScheduler` creates a stable `PendingIntent` and chooses the correct `AlarmManager` method for the installed Android version.
5. `ReminderReceiver` receives the broadcast and displays it through a high-priority notification channel.

The orchestration remains deliberately straightforward:

```csharp
var errors = draft.Validate(DateTimeOffset.Now);
if (errors.Count > 0)
{
    throw new ReminderValidationException(errors);
}

var reminder = draft.ToReminder();
await scheduler.ScheduleAsync(reminder, cancellationToken);

try
{
    var items = (await store.LoadAsync(cancellationToken)).ToList();
    items.Add(reminder);
    await store.SaveAsync(items, cancellationToken);
}
catch
{
    await scheduler.CancelAsync(reminder.Id, CancellationToken.None);
    throw;
}
```

### Other examples

- `ReminderRecurrence` advances daily, weekly and monthly reminders without skipping overdue occurrences or drifting at the end of a month.
- `JsonReminderStore` protects local state with a semaphore and replaces the data file atomically after serialization.
- `WeatherClient` maps a focused Open-Meteo response into an app-facing weather model.
- `StopwatchTimer` keeps accumulated time and lap splits independent from the UI refresh interval.
- `StopwatchViewModel` manages several running timers with one lightweight dispatcher timer.
- `ThemeManager` applies and remembers the user's light or dark appearance preference.
- `AppBootstrap` shows how platform contracts, services, view models and pages are registered with dependency injection.

The sample project can be checked with:

```bash
dotnet build samples/TamVakti.Sample.csproj -c Release
```

These examples are intentionally selected. The complete production UI, background stopwatch service, localization catalog, store configuration and proprietary application logic are not published here.

## Repository scope

This is a product showcase with selected code samples, not the full open-source repository of Tam Vakti.

- Production source code and proprietary application logic are not included.
- Signing keys, API credentials, APK/AAB packages and build outputs are excluded.
- Code under `samples/` is independent and safe to reuse under the MIT License.
- Screenshots, the app icon and other brand assets are provided only to present the product.

## Privacy

The app's current privacy policy is available in [PRIVACY.md](PRIVACY.md). The published policy is kept in Turkish to match the current store listing.

## License

Source code under `samples/` is available under the [MIT License](LICENSE). The Tam Vakti name, app icon, screenshots and other visual assets are not covered by the MIT License. See [ASSETS_LICENSE.md](ASSETS_LICENSE.md) for details.

---

<div align="center">
  <a href="https://play.google.com/store/apps/details?id=com.msds.tamvakti">View Tam Vakti on Google Play</a>
</div>
