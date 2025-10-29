using SD.Core.Shared.Models;
using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class ComboBoxValueToLoadCaseConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) 
            return null;
        if (value is LoadCaseCombination)
        {
            (value as LoadCaseCombination).Include = true;
            return value;
        }
        return null;
    }
}