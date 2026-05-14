using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SD.UI.Behaviors;

public class ExcelSingleClickEditBehavior : Behavior<System.Windows.Controls.DataGrid>
{
    private System.Windows.Controls.DataGridCell mouseDownCell;
    private System.Windows.Point mouseDownPoint;
    private bool isDragSelection;

    protected override void OnAttached()
    {
        AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        AssociatedObject.MouseMove += OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        AssociatedObject.MouseMove -= OnMouseMove;
        AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
    }

    private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        mouseDownCell = FindParent<System.Windows.Controls.DataGridCell>(e.OriginalSource as DependencyObject);
        mouseDownPoint = e.GetPosition(AssociatedObject);
        isDragSelection = false;
    }

    private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (mouseDownCell == null || e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        var currentPoint = e.GetPosition(AssociatedObject);
        if (Math.Abs(currentPoint.X - mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(currentPoint.Y - mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
            isDragSelection = true;
        }
    }

    private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var cell = FindParent<System.Windows.Controls.DataGridCell>(e.OriginalSource as DependencyObject);
        if (!isDragSelection &&
            mouseDownCell != null &&
            ReferenceEquals(cell, mouseDownCell) &&
            cell != null &&
            !cell.IsEditing &&
            cell.IsSelected &&
            Keyboard.Modifiers == ModifierKeys.None)
        {
            var grid = AssociatedObject;
            grid.CurrentCell = new DataGridCellInfo(cell);
            grid.BeginEdit();
            e.Handled = true;
        }

        mouseDownCell = null;
        isDragSelection = false;
    }

    private T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T parent)
                return parent;

            child = VisualTreeHelper.GetParent(child);
        }
        return null;
    }
}