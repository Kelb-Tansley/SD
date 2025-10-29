using SD.UI.Helpers;
using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class DoubleToUnitConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object unit, CultureInfo culture)
    {
        // Conversion closures
        var NTokN = new Func<double, double>(n => n / 1000); // Convert N to kN
        var NmmTokNm = new Func<double, double>(nmm => nmm / 1000000); // Convert Nmm to kNm

        if (value is double doubleValue && unit is string unitValue)
        {
            if (unitValue == "kN")
            {
                // Convert doubleValue from N to kN
                doubleValue = NTokN(doubleValue);
            }
            else if (unitValue == "kNm")
            {
                // Convert doubleValue from Nmm to kNm
                doubleValue = NmmTokNm(doubleValue);
            }

            var x = Math.Truncate(doubleValue * 100) / 100;
            return x.ToString($"0.{ConverterHelper.GetHashes(2)}");
        }

        return value; // Return the original value if conversion fails
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Not needed for this
    }
}
