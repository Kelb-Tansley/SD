using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace SD.UI.Converters;
public class IconToImageSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        try
        {
            string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)
                 + "\\Styles\\Images\\Icos\\" + value;

            using Icon ico = Icon.ExtractAssociatedIcon(path);
            return Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        catch (Exception) { return null; }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotImplementedException();
    }
}