using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace SD.UI.Converters;
public class ComboBoxValueToBoolConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || !(bool)value)
            return "Default";

        return "Calculate";
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) 
            return false;
        if ((value as ComboBoxItem)?.Content?.ToString() == "Calculate")
            return true;
        return false;
    }
}