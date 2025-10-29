using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SD.UI.Converters;
public class ObjectNullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        return (value == null) ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        return value;
    }
}