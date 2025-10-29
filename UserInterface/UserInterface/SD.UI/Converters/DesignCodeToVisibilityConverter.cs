using System.Globalization;
using System.Windows.Data;
using System.Windows;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Constants;

namespace SD.UI.Converters;
public class DesignCodeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        var parString = parameter?.ToString();
        var parseResults = Enum.TryParse(parString, out DesignCode matchingDesignType);
        if (!parseResults)
            matchingDesignType = DesignCode.SANS;

        var designCode = value?.ToString();
        if (string.IsNullOrWhiteSpace(designCode))
            return Visibility.Collapsed;

        switch (matchingDesignType)
        {
            case DesignCode.SANS:
                return DesignCodes.IsSans(designCode) ? Visibility.Visible : Visibility.Collapsed;
            case DesignCode.AS:
                return DesignCodes.IsAs(designCode) ? Visibility.Visible : Visibility.Collapsed;
            default:
                break;
        }
        throw new NotImplementedException(nameof(Convert));
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotImplementedException(nameof(ConvertBack));
    }
}