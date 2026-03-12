using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SD.UI.Converters;
public class ListCountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable<object> collection)
        {
            return collection.Any() ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Not needed for this
    }
}
