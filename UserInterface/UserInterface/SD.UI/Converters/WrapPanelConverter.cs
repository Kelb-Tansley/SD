using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class WrapPanelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var parseResult = Double.TryParse(value?.ToString(), out var width);
        if (!parseResult)
            return 0;

        return width;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}