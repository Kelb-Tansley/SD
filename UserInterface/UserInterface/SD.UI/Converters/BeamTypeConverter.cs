using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.Sans;
using System.Globalization;
using System.Windows.Data;

namespace SD.UI.Converters;
public class BeamTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value == null)
            return false;

        if (value is SansUlsResult result)
            return result.Beam.Section.SectionType == SectionType.IorH || result.Beam.Section.SectionType == SectionType.LipChannel;

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}