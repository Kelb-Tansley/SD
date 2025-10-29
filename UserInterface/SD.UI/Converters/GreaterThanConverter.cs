using SD.UI.Helpers;
using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class GreaterThanConverter : IValueConverter
{
    public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
    {
        //if (limit is double capacity)
        //{
        //    if (value is double doubleValue)
        //    {
        //        return doubleValue > capacity;
        //    }

        //    if (value is string stringValue)
        //    {
        //        var newDouble = System.Convert.ToDouble(stringValue.Replace("%", ""));
        //        return newDouble > 100;
        //    }
        //}

        // TODO

        return true;
    }
    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
