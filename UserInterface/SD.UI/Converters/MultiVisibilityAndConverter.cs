using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SD.UI.Converters;

/// <summary>
/// Returns Collapsed if any of the bound Visibility values is Collapsed; otherwise Visible.
/// </summary>
public class MultiVisibilityAndConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        foreach (var value in values)
        {
            if (value is Visibility visibility && visibility == Visibility.Collapsed)
                return Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
