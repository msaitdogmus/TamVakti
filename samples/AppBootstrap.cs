using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
#if ANDROID
using TamVakti.Sample.Platforms.Android;
#endif
using TamVakti.Sample.Services;
using TamVakti.Sample.ViewModels;
using TamVakti.Sample.Views;

namespace TamVakti.Sample;

public static class AppBootstrap
{
    public static MauiAppBuilder AddTamVaktiSample(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IReminderStore, JsonReminderStore>();
        builder.Services.AddSingleton<ReminderService>();
        builder.Services.AddSingleton(new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        });
        builder.Services.AddSingleton<WeatherClient>();
        builder.Services.AddSingleton<ThemeManager>();
        builder.Services.AddTransient<ReminderListViewModel>();
        builder.Services.AddTransient<ReminderEditorViewModel>();
        builder.Services.AddTransient<StopwatchViewModel>();
        builder.Services.AddTransient<ReminderEditorPage>();

#if ANDROID
        builder.Services.AddSingleton<IReminderScheduler, AndroidReminderScheduler>();
#else
        builder.Services.AddSingleton<IReminderScheduler, NoOpReminderScheduler>();
#endif

        return builder;
    }
}
