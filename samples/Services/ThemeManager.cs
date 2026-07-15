using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace TamVakti.Sample.Services;

public sealed class ThemeManager
{
    private const string PreferenceKey = "appearance.dark_mode";

    public bool IsDark { get; private set; } = true;

    public event EventHandler? Changed;

    public void Load()
    {
        Apply(Preferences.Get(PreferenceKey, true), save: false);
    }

    public void Toggle()
    {
        Apply(!IsDark);
    }

    public void Apply(bool dark, bool save = true)
    {
        IsDark = dark;

        if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = dark
                ? AppTheme.Dark
                : AppTheme.Light;
        }

        if (save)
        {
            Preferences.Set(PreferenceKey, dark);
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }
}
