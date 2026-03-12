using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SD.UI.Converters;

public class BooleanToWindowStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is null)
            return WindowState.Normal;
        return (value is bool boolean && boolean) ? WindowState.Maximized : WindowState.Minimized;
    }
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is WindowState.Maximized)
            return true;
        else if (value is WindowState.Minimized)
            return false;

        return null;
    }
}