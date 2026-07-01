using System.Globalization;
using System.Windows.Data;
using System.Windows;
using SD.Core.Shared.Enum;

namespace SD.UI.Converters;

public class DesignTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DesignType designType)
            return Visibility.Collapsed;

        return designType == DesignType.BendingAxial || designType == DesignType.Bending
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(nameof(ConvertBack));
    }
}
