using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SD.UI.Controls;

public class ExpandingCard : ContentControl
{
    private double _originalHeight = 48;
    private FrameworkElement? _hostContainer;


    public static readonly DependencyProperty ExpanderTitleProperty = DependencyProperty.Register(nameof(ExpanderTitle),
                                                                                                  typeof(string),
                                                                                                  typeof(ExpandingCard),
                                                                                                  new PropertyMetadata(string.Empty));

    public string ExpanderTitle
    {
        get => (string)GetValue(ExpanderTitleProperty);
        set => SetValue(ExpanderTitleProperty, value);
    }

    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded),
                                                                                               typeof(bool),
                                                                                               typeof(ExpandingCard),
                                                                                               new PropertyMetadata(false));

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    static ExpandingCard()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ExpandingCard), new FrameworkPropertyMetadata(typeof(ExpandingCard)));
    }

    public ExpandingCard()
    {
        //Loaded += ExpandingCard_Loaded;
        //Unloaded += ExpandingCard_Unloaded;
    }

    private void ExpandingCard_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_hostContainer != null)
        {
            _hostContainer.SizeChanged -= HostContainer_SizeChanged;
        }
    }

    private void ExpandingCard_Loaded(object sender, RoutedEventArgs e)
    {
        // find nearest layout container to use available height
        _hostContainer = FindAncestor<FrameworkElement>(this);
        if (_hostContainer != null)
        {
            _hostContainer.SizeChanged += HostContainer_SizeChanged;
        }

        // capture original height (if set) otherwise keep NaN
        if (!Double.IsNaN(this.ActualHeight) && this.ActualHeight > 0)
        {
            _originalHeight = this.ActualHeight;
        }
    }

    private void HostContainer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (IsExpanded)
        {
            ApplyExpandedHeight();
        }
    }

    private static T FindAncestor<T>(DependencyObject from) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(from);
        while (parent != null && !(parent is T))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as T;
    }

    private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ExpandingCard)d;
        if ((bool)e.NewValue)
            ctrl.ApplyExpandedHeight();
        else
            ctrl.RestoreOriginalHeight();
    }

    private void ApplyExpandedHeight()
    {
        // store original height if not stored
        if (Double.IsNaN(_originalHeight) || _originalHeight == 0)
        {
            if (!Double.IsNaN(this.Height) && this.Height > 0)
                _originalHeight = this.Height;
            else if (this.ActualHeight > 0)
                _originalHeight = this.ActualHeight;
            else
                _originalHeight = Double.NaN;
        }

        double available = Double.NaN;
        if (_hostContainer != null)
        {
            // attempt to use the host container's actual height minus margin
            available = Math.Max(0, _hostContainer.ActualHeight - 24);
        }

        if (!Double.IsNaN(available) && available > 0)
        {
            this.Height = available;
            this.VerticalAlignment = VerticalAlignment.Top;
        }
        else
        {
            // fallback to stretch
            this.Height = Double.NaN;
            this.VerticalAlignment = VerticalAlignment.Stretch;
        }
    }

    private void RestoreOriginalHeight()
    {
        if (!Double.IsNaN(_originalHeight) && _originalHeight > 0)
        {
            this.Height = _originalHeight;
        }
        else
        {
            this.Height = 48; // Auto
        }

        this.VerticalAlignment = VerticalAlignment.Top;
    }

}
