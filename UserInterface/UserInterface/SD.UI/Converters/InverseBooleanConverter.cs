using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is bool boolean)
            return !boolean;
        else return false;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is bool boolean && boolean)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
}