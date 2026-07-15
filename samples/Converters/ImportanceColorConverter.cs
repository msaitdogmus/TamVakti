using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TamVakti.Sample.Models;

namespace TamVakti.Sample.Converters;

public sealed class ImportanceColorConverter : IValueConverter
{
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        return value switch
        {
            ImportanceLevel.Low => Color.FromArgb("#93A29A"),
            ImportanceLevel.Normal => Color.FromArgb("#16C784"),
            ImportanceLevel.High => Color.FromArgb("#F2778A"),
            _ => Colors.Gray
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) => throw new NotSupportedException();
}
