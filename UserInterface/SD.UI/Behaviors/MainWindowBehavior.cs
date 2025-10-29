using SD.UI.Helpers;
using System.Windows;

namespace SD.UI.Behaviors
{
    public static class MainWindowBehavior
    {
        public static int GetWindowResizer(DependencyObject obj)
        {
            return (int)obj.GetValue(WindowResizerProperty);
        }

        public static void SetWindowResizer(DependencyObject obj, string value)
        {
            obj.SetValue(WindowResizerProperty, value);
        }

        public static readonly DependencyProperty WindowResizerProperty =
            DependencyProperty.RegisterAttached("WindowResizer", typeof(string), typeof(MainWindowBehavior), new PropertyMetadata(null, OnWindowBehavior));

        private static void OnWindowBehavior(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window element)
            {
                element.Initialized += (s, e2) =>
                {
                    var resizerType = e.NewValue?.ToString();

                    if (element.DataContext == null)
                        element.DataContextChanged += (s, e) => InitializeWindowResizer(resizerType, element);
                    else
                        InitializeWindowResizer(resizerType, element);
                };
            }
        }

        private static void InitializeWindowResizer(string? e, Window element)
        {
            var viewModel = element.DataContext;
            if (viewModel == null)
                return;

            if (!string.IsNullOrWhiteSpace(e))
                viewModel.GetType()?.GetProperty(e)?.SetValue(viewModel, new WindowResizer(element));
        }
    }
}
