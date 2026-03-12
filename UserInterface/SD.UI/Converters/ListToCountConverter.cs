using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class ListToCountConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable<object> collection)
        {
            return collection.Count();
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Not needed for this
    }
}
