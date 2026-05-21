using Microsoft.Xaml.Behaviors;
using SD.Core.Shared.Models;
using System.Windows;
using System.Windows.Controls;

namespace SD.UI.UltimateLimitState.Behaviors;

public class DataGridSelectedCellBehavior : Behavior<DataGrid>
{
    public static readonly DependencyProperty SelectedCellProperty = DependencyProperty.Register(
        nameof(SelectedCell),
        typeof(UlsResult),
        typeof(DataGridSelectedCellBehavior),
        new FrameworkPropertyMetadata(default(UlsResult), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedCellChanged)
    );

    public UlsResult SelectedCell
    {
        get => (UlsResult)GetValue(SelectedCellProperty);
        set => SetValue(SelectedCellProperty, value);
    }

    private static void OnSelectedCellChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var behavior = (DataGridSelectedCellBehavior)d;
        behavior.OnSelectedCellChanged((UlsResult?)e.OldValue, (UlsResult?)e.NewValue);
    }

    protected virtual void OnSelectedCellChanged(UlsResult? oldValue, UlsResult? newValue)
    {
        if (AssociatedObject == null)
            return;

        if (newValue != null)
        {
            var firstColumn = AssociatedObject.Columns.FirstOrDefault(c => c.Visibility == System.Windows.Visibility.Visible);
            if (firstColumn != null)
            {
                AssociatedObject.SelectedCells.Clear();

                var selectedCell = new DataGridCellInfo(newValue, firstColumn);
                AssociatedObject.SelectedCells.Add(selectedCell);
                AssociatedObject.CurrentCell = selectedCell;
                AssociatedObject.ScrollIntoView(newValue);
            }
        }
    }
}