using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace SD.UI.Main.Behaviors;

public class FlickerOnVisibilityBehavior : Behavior<FrameworkElement>
{
    private bool _isAnimationRunning = false;

    public static readonly DependencyProperty StoryboardNameProperty =
        DependencyProperty.Register(
            nameof(StoryboardName),
            typeof(string),
            typeof(FlickerOnVisibilityBehavior),
            new PropertyMetadata(null));

    public string StoryboardName
    {
        get => (string)GetValue(StoryboardNameProperty);
        set => SetValue(StoryboardNameProperty, value);
    }

    public static readonly DependencyProperty TargetElementProperty =
        DependencyProperty.Register(
            nameof(TargetElement),
            typeof(FrameworkElement),
            typeof(FlickerOnVisibilityBehavior),
            new PropertyMetadata(null));

    public FrameworkElement TargetElement
    {
        get => (FrameworkElement)GetValue(TargetElementProperty);
        set => SetValue(TargetElementProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject.Visibility == Visibility.Visible)
        {
            TriggerAnimation();
        }

        DependencyPropertyDescriptor visibilityDescriptor =
            DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, typeof(UIElement));
        visibilityDescriptor.AddValueChanged(AssociatedObject, OnVisibilityChanged);
    }

    protected override void OnDetaching()
    {
        DependencyPropertyDescriptor visibilityDescriptor =
            DependencyPropertyDescriptor.FromProperty(UIElement.VisibilityProperty, typeof(UIElement));
        visibilityDescriptor.RemoveValueChanged(AssociatedObject, OnVisibilityChanged);

        base.OnDetaching();
    }

    private void OnVisibilityChanged(object sender, EventArgs e)
    {
        if (AssociatedObject.Visibility == Visibility.Visible && !_isAnimationRunning)
        {
            TriggerAnimation();
        }
        else if (AssociatedObject.Visibility == Visibility.Collapsed)
        {
            _isAnimationRunning = false;
        }
    }

    private void TriggerAnimation()
    {
        _isAnimationRunning = true;

        if (!string.IsNullOrEmpty(StoryboardName) && TargetElement != null)
        {
            var storyboard = FindStoryboard();

            if (storyboard != null)
            {
                var clonedStoryboard = storyboard.Clone();
                clonedStoryboard.Completed += (_, _) =>
                {
                    _isAnimationRunning = false;
                };
                clonedStoryboard.Begin(TargetElement);
                return;
            }
        }

        _isAnimationRunning = false;
    }

    private Storyboard? FindStoryboard()
    {
        if (AssociatedObject.Resources.Contains(StoryboardName))
        {
            return AssociatedObject.Resources[StoryboardName] as Storyboard;
        }

        FrameworkElement? current = AssociatedObject;
        while (current != null)
        {
            if (current is UserControl && current.Resources.Contains(StoryboardName))
            {
                return current.Resources[StoryboardName] as Storyboard;
            }

            current = GetParent(current);
        }

        if (Application.Current?.Resources.Contains(StoryboardName) == true)
        {
            return Application.Current.Resources[StoryboardName] as Storyboard;
        }

        return null;
    }

    private static FrameworkElement? GetParent(FrameworkElement element)
    {
        return element.Parent as FrameworkElement
            ?? LogicalTreeHelper.GetParent(element) as FrameworkElement
            ?? VisualTreeHelper.GetParent(element) as FrameworkElement;
    }
}
