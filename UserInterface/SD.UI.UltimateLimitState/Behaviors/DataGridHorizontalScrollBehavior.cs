using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace SD.UI.UltimateLimitState.Behaviors;

public class DataGridHorizontalScrollBehavior : Behavior<DataGrid>
{
    private HwndSource? _hwndSource;
    private IntPtr _hwnd = IntPtr.Zero;
    private const int WM_MOUSEHWHEEL = 0x020E;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        AssociatedObject.Loaded += OnLoaded;
        AssociatedObject.Unloaded += OnUnloaded;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        AssociatedObject.Loaded -= OnLoaded;
        AssociatedObject.Unloaded -= OnUnloaded;
        RemoveHwndHook();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachHwndHook();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        RemoveHwndHook();
    }

    private void AttachHwndHook()
    {
        if (_hwndSource != null)
            return;
        var window = Window.GetWindow(AssociatedObject);
        if (window == null)
            return;
        var interopHelper = new WindowInteropHelper(window);
        _hwnd = interopHelper.Handle;
        _hwndSource = HwndSource.FromHwnd(_hwnd);
        _hwndSource?.AddHook(WndProc);
    }

    private void RemoveHwndHook()
    {
        _hwndSource?.RemoveHook(WndProc);
        _hwndSource = null;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_MOUSEHWHEEL && AssociatedObject.IsVisible)
        {
            int delta = (short)((wParam.ToInt64() >> 16) & 0xffff);
            var scrollViewer = FindScrollViewer(AssociatedObject);
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + delta);
                handled = true;
            }
        }
        return IntPtr.Zero;
    }

    private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (AssociatedObject == null)
            return;

        var scrollViewer = FindScrollViewer(AssociatedObject);
        if (scrollViewer == null)
            return;

        // Shift+Wheel for horizontal scroll
        if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
        {
            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.Delta);
            e.Handled = true;
        }
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject d)
    {
        if (d is ScrollViewer sv)
            return sv;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
        {
            var child = VisualTreeHelper.GetChild(d, i);
            var result = FindScrollViewer(child);
            if (result != null)
                return result;
        }
        return null;
    }
}