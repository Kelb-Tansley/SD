using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace SD.UI.UltimateLimitState.Behaviors;

public class DataGridScrollIntoViewBehavior : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
    }

    private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (AssociatedObject.SelectedItem != null)
        {
            AssociatedObject.ScrollIntoView(AssociatedObject.SelectedItem);
        }
    }
}
