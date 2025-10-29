using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SD.UI.Converters;
public class ComboBoxValueToStringConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value as ComboBoxItem)?.Content?.ToString();
    }
}