using SD.UI.Helpers;
using System.Globalization;
using System.Reflection.Metadata;
using System.Windows.Data;

namespace SD.UI.Converters;
public class DoubleTwoDecimalsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object decimalPlaces, CultureInfo culture)
    {
        if (value is double doubleValue)
            return doubleValue.ToString($"0.{ConverterHelper.GetHashes(decimalPlaces)}");

        return value; // Return the original value if conversion fails
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Not needed for this example
    }
}
