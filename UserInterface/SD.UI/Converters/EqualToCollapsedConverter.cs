using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SD.UI.Converters;
public class EqualToCollapsedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string paramStr && int.TryParse(paramStr, out int paramInt))
            return value is int intValue && intValue == paramInt ? Visibility.Collapsed : Visibility.Visible;

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
