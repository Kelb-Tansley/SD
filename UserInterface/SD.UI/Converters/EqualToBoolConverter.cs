using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;

public class EqualToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string paramStr && int.TryParse(paramStr, out int paramInt))
            return value is int intValue && intValue == paramInt;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
