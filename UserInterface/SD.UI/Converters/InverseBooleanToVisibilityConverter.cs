using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace SD.UI.Converters;
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        return (value is bool boolean && !boolean) ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
