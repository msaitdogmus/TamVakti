using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using TamVakti.Sample.Services;
using TamVakti.Sample.ViewModels;

namespace TamVakti.Sample;

public static class AppBootstrap
{
    public static MauiAppBuilder AddTamVaktiSample(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IReminderStore, JsonReminderStore>();
        builder.Services.AddSingleton<IReminderScheduler, NoOpReminderScheduler>();
        builder.Services.AddTransient<ReminderListViewModel>();

        return builder;
    }
}
