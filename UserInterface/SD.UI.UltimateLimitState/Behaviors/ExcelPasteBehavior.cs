using Microsoft.Xaml.Behaviors;
using System.Text;
using System.Windows.Input;

namespace SD.UI.UltimateLimitState.Behaviors;

public class ExcelPasteBehavior : Behavior<System.Windows.Controls.DataGrid>
{
    protected override void OnAttached()
    {
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
    }

    private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if ((e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control) ||
            (e.Key == Key.Insert && Keyboard.Modifiers == ModifierKeys.Control))
        {
            if (CopySelectedCellsWithoutHeaders(AssociatedObject))
            {
                e.Handled = true;
            }
        }

        // Let view-level handlers process Ctrl+V so specialized paste logic remains intact.
    }

    private static bool CopySelectedCellsWithoutHeaders(System.Windows.Controls.DataGrid grid)
    {
        if (grid.SelectedCells.Count == 0)
            return false;

        var selected = grid.SelectedCells
            .Select(c => new
            {
                Cell = c,
                RowIndex = grid.Items.IndexOf(c.Item),
                ColumnIndex = c.Column.DisplayIndex
            })
            .Where(x => x.RowIndex >= 0)
            .ToList();

        if (selected.Count == 0)
            return false;

        var rowIndexes = selected.Select(x => x.RowIndex).Distinct().OrderBy(x => x).ToList();
        var columnIndexes = selected.Select(x => x.ColumnIndex).Distinct().OrderBy(x => x).ToList();

        var values = new Dictionary<(int Row, int Col), string>();
        foreach (var entry in selected)
        {
            values[(entry.RowIndex, entry.ColumnIndex)] = GetCellText(entry.Cell);
        }

        var sb = new StringBuilder();

        for (var r = 0; r < rowIndexes.Count; r++)
        {
            for (var c = 0; c < columnIndexes.Count; c++)
            {
                if (values.TryGetValue((rowIndexes[r], columnIndexes[c]), out var value))
                {
                    sb.Append(value);
                }

                if (c < columnIndexes.Count - 1)
                    sb.Append('\t');
            }

            if (r < rowIndexes.Count - 1)
                sb.AppendLine();
        }

        var text = sb.ToString();
        if (string.IsNullOrEmpty(text))
            return false;

        var dataObject = new System.Windows.DataObject();
        dataObject.SetData(System.Windows.DataFormats.UnicodeText, text);
        dataObject.SetData(System.Windows.DataFormats.Text, text);
        System.Windows.Clipboard.SetDataObject(dataObject, true);
        return true;
    }

    private static string GetCellText(System.Windows.Controls.DataGridCellInfo cellInfo)
    {
        // Try visual container first (works for visible/rendered rows).
        var content = cellInfo.Column.GetCellContent(cellInfo.Item);

        if (content is System.Windows.Controls.TextBlock textBlock)
            return textBlock.Text ?? string.Empty;

        if (content is System.Windows.Controls.TextBox textBox)
            return textBox.Text ?? string.Empty;

        if (content is System.Windows.Controls.CheckBox checkBox)
            return checkBox.IsChecked == true ? "True" : "False";

        // Fallback: extract value directly from the data item via the column binding.
        // This handles virtualized rows whose visual containers are not in the tree.
        if (cellInfo.Column is System.Windows.Controls.DataGridBoundColumn boundColumn)
        {
            var binding = boundColumn.Binding as System.Windows.Data.Binding;
            if (binding?.Path?.Path != null && cellInfo.Item != null)
            {
                try
                {
                    var value = GetPropertyValue(cellInfo.Item, binding.Path.Path);
                    return value?.ToString() ?? string.Empty;
                }
                catch
                {
                    // ignore reflection errors
                }
            }
        }

        return string.Empty;
    }

    private static object GetPropertyValue(object item, string propertyPath)
    {
        var current = item;
        foreach (var part in propertyPath.Split('.'))
        {
            if (current == null)
                return null;
            var prop = current.GetType().GetProperty(part);
            if (prop == null)
                return null;
            current = prop.GetValue(current);
        }
        return current;
    }
}