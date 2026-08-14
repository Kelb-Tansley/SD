using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Button = System.Windows.Controls.Button;
using MaterialDesignThemes.Wpf;
using SD.UI.Helpers;

namespace SD.UI.Behaviors
{
    public class NavigationPanelToggleBehavior : Behavior<Grid>
    {
        private const int _animationDelay = 300; // Milliseconds
        private double _previousWidth = 300; // Pixels
        private double _savedCol0Width = 0;
        private Button? _toggleButton;
        private PackIcon? _toggleIcon;

        public double MinWidth { get; set; }
        public string ToggleButtonName { get; set; } = "NavToggleButton";
        public string ToggleIconName { get; set; } = "NavToggleIcon";

        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register(
            nameof(IsCollapsed), typeof(bool), typeof(NavigationPanelToggleBehavior),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCollapsedChanged));

        public bool IsCollapsed
        {
            get => (bool)GetValue(IsCollapsedProperty);
            set => SetValue(IsCollapsedProperty, value);
        }

        public static readonly DependencyProperty IsForceHiddenProperty = DependencyProperty.Register(
            nameof(IsForceHidden), typeof(bool), typeof(NavigationPanelToggleBehavior),
            new FrameworkPropertyMetadata(false, OnIsForceHiddenChanged));

        public bool IsForceHidden
        {
            get => (bool)GetValue(IsForceHiddenProperty);
            set => SetValue(IsForceHiddenProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.Initialized += (s, e) => InitializeControls();
            base.OnAttached();
        }

        private void InitializeControls()
        {
            _toggleButton = AssociatedObject.FindName(ToggleButtonName) as Button ?? FindChild<Button>(AssociatedObject, ToggleButtonName);
            if (_toggleButton != null)
                _toggleButton.Click += ToggleButton_Click;

            _toggleIcon = AssociatedObject.FindName(ToggleIconName) as PackIcon ?? FindChild<PackIcon>(AssociatedObject, ToggleIconName);

            // initialize visual state from bound value
            UpdateVisualState(IsCollapsed, false);
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Initialized -= (s, e) => InitializeControls();
            if (_toggleButton != null)
                _toggleButton.Click -= ToggleButton_Click;
            base.OnDetaching();
        }

        private void ToggleButton_Click(object? sender, RoutedEventArgs e)
        {
            IsCollapsed = !IsCollapsed;
        }

        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationPanelToggleBehavior beh)
            {
                bool newVal = (bool)e.NewValue;
                beh.UpdateVisualState(newVal, true);
            }
        }

        private static void OnIsForceHiddenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is NavigationPanelToggleBehavior beh)
                beh.ApplyForceHidden((bool)e.NewValue);
        }

        private void ApplyForceHidden(bool hide)
        {
            if (AssociatedObject == null || AssociatedObject.ColumnDefinitions.Count < 2)
                return;

            var col0 = AssociatedObject.ColumnDefinitions[0];

            if (hide)
            {
                _savedCol0Width = col0.ActualWidth > 0 ? col0.ActualWidth : _previousWidth;
                col0.BeginAnimation(ColumnDefinition.WidthProperty, null);
                col0.Width = new GridLength(0, GridUnitType.Pixel);
            }
            else
            {
                col0.Width = new GridLength(_savedCol0Width > 0 ? _savedCol0Width : _previousWidth, GridUnitType.Pixel);
                // Re-apply collapsed state so the toggle behavior stays in sync
                UpdateVisualState(IsCollapsed, false);
            }
        }

        private void UpdateVisualState(bool collapse, bool animate)
        {
            if (AssociatedObject == null || AssociatedObject.ColumnDefinitions.Count == 0)
                return;

            var leftColumn = AssociatedObject.ColumnDefinitions[0];

            if (collapse)
            {
                _previousWidth = leftColumn.ActualWidth > 0 ? leftColumn.ActualWidth : _previousWidth;
                if (animate)
                {
                    var anim = new GridLengthAnimation
                    {
                        From = new GridLength(_previousWidth, GridUnitType.Pixel),
                        To = new GridLength(MinWidth, GridUnitType.Pixel),
                        Duration = new Duration(TimeSpan.FromMilliseconds(_animationDelay))
                    };
                    anim.Completed += (s, ev) => ClearAnimation(leftColumn, anim.To);
                    leftColumn.BeginAnimation(ColumnDefinition.WidthProperty, anim);
                }
                else
                {
                    leftColumn.Width = new GridLength(MinWidth, GridUnitType.Pixel);
                }
            }
            else
            {
                if (animate)
                {
                    var anim = new GridLengthAnimation
                    {
                        From = new GridLength(MinWidth, GridUnitType.Pixel),
                        To = new GridLength(_previousWidth, GridUnitType.Pixel),
                        Duration = new Duration(TimeSpan.FromMilliseconds(200))
                    };
                    anim.Completed += (s, ev) => ClearAnimation(leftColumn, anim.To);
                    leftColumn.BeginAnimation(ColumnDefinition.WidthProperty, anim);
                }
                else
                {
                    leftColumn.Width = new GridLength(_previousWidth, GridUnitType.Pixel);
                }
            }

            //if (_toggleIcon != null)
            //    _toggleIcon.Kind = collapse ? PackIconKind.UnfoldMoreVertical : PackIconKind.UnfoldMoreVertical;
        }

        private static void ClearAnimation(ColumnDefinition column, GridLength finalWidth)
        {
            // Clear the animation and explicitly set the final width so GridSplitter can resize the column
            column.BeginAnimation(ColumnDefinition.WidthProperty, null);
            column.Width = finalWidth;
        }

        private static T? FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null)
                return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T fe && fe.Name == name)
                    return fe;

                var result = FindChild<T>(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}