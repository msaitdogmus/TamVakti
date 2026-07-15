using Microsoft.Maui.Controls;
using TamVakti.Sample.Models;
using TamVakti.Sample.ViewModels;

namespace TamVakti.Sample.Views;

public partial class ReminderEditorPage : ContentPage
{
    private readonly ReminderEditorViewModel viewModel;

    public ReminderEditorPage(ReminderEditorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = this.viewModel = viewModel;
    }

    private void OnImportanceClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: ImportanceLevel level })
        {
            viewModel.SetImportance(level);
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (await viewModel.SaveAsync() && Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
    }
}
