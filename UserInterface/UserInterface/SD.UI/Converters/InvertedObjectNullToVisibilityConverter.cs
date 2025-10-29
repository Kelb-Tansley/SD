using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SD.UI.Converters;
public class InvertedObjectNullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        return (value == null) ? Visibility.Collapsed : Visibility.Visible;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        return value;
    }
}