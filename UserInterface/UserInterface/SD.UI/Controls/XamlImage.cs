using System.Windows;

namespace SD.UI.Controls;
public class XamlImage : System.Windows.Controls.Control
{
    static XamlImage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlImage),
            new FrameworkPropertyMetadata(typeof(XamlImage)));

        IsTabStopProperty.OverrideMetadata(typeof(XamlImage),
            new FrameworkPropertyMetadata(false));
    }
}
