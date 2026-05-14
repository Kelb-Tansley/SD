using System.Windows;

namespace SD.UI.Behaviors;

public static class ExcelDataGridTextColumnBehavior
{
    public static readonly DependencyProperty EnableFillHandleProperty =
        DependencyProperty.RegisterAttached(
            "EnableFillHandle",
            typeof(bool),
            typeof(ExcelDataGridTextColumnBehavior),
            new PropertyMetadata(false));

    public static void SetEnableFillHandle(DependencyObject element, bool value)
        => element.SetValue(EnableFillHandleProperty, value);

    public static bool GetEnableFillHandle(DependencyObject element)
        => (bool)element.GetValue(EnableFillHandleProperty);
}