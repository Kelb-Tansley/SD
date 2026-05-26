using Microsoft.Xaml.Behaviors;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SD.UI.UltimateLimitState.Behaviors;

public class UlsDataGridKeyboardBehavior : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        AssociatedObject.PreparingCellForEdit += PreparingCellForEdit;
        AssociatedObject.CurrentCellChanged += OnCurrentCellChanged;
    }

    private string? _editingHeader;
    private TextBox? _editingTextBox;
    private UlsResult? _editingRowItem;

    private void PreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (e.Column.Header is string header && IsKColumn(header))
            _editingHeader = header;
        else
            _editingHeader = null;

        _editingTextBox = e.EditingElement as TextBox;
        _editingRowItem = e.Row.Item as UlsResult;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        AssociatedObject.CurrentCellChanged -= OnCurrentCellChanged;
        AssociatedObject.PreparingCellForEdit -= PreparingCellForEdit;
    }

    private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.V && System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Control))
        {
            if (PasteKValues(AssociatedObject))
                e.Handled = true;
            return;
        }

        if (e.Key == System.Windows.Input.Key.Down && System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift))
        {
            if (FillDown(AssociatedObject))
                e.Handled = true;
            return;
        }

        if (e.Key == System.Windows.Input.Key.Right && System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift))
        {
            if (FillRight(AssociatedObject))
                e.Handled = true;
        }
    }

    private void OnCurrentCellChanged(object? sender, EventArgs e)
    {
        // If we didn’t capture a valid edit context, ignore
        if (_editingHeader is null ||
            _editingTextBox is null ||
            _editingRowItem?.Beam?.BeamChain is not BeamChain editedChain)
            return;

        // Parse the new value
        if (!TryParseDouble(_editingTextBox.Text, out var newValue))
            return;

        // Find all editable columns with the same header
        var editableColumns = AssociatedObject.Columns
            .Where(c => c.Header is string h && h == _editingHeader)
            .ToList();

        var touchedChains = new HashSet<BeamChain> { editedChain };

        // Defer until DataGrid has fully committed the edit
        AssociatedObject.Dispatcher.BeginInvoke(
            DispatcherPriority.Background,
            () => RefreshRowsSharingChains(AssociatedObject, editableColumns, touchedChains)
        );
    }

    private static bool PasteKValues(DataGrid dataGrid)
    {
        if (!System.Windows.Clipboard.ContainsText())
            return false;

        string clipboardText = System.Windows.Clipboard.GetText();
        if (string.IsNullOrWhiteSpace(clipboardText))
            return false;

        var rowValues = clipboardText.Replace("\r\n", "\n").Replace('\r', '\n')
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(row => row.Split('\t'))
            .ToList();

        var editableColumns = dataGrid.Columns
            .Where(column => column.Header is string header && IsKColumn(header))
            .OrderBy(column => column.DisplayIndex)
            .ToList();

        if (editableColumns.Count == 0 || rowValues.Count == 0)
            return false;

        var startColumn = dataGrid.CurrentCell.Column;
        var startColumnIndex = startColumn is null ? 0 : editableColumns.FindIndex(column => column == startColumn);
        if (startColumnIndex < 0)
            return false;

        var startRowIndex = dataGrid.CurrentCell.Item is null ? -1 : dataGrid.Items.IndexOf(dataGrid.CurrentCell.Item);
        if (startRowIndex < 0)
            return false;

        if (rowValues.Count > 0)
        {
            var firstRow = rowValues[0];
            var comparableCount = Math.Min(firstRow.Length, editableColumns.Count - startColumnIndex);
            var headerMatches = 0;

            for (var columnOffset = 0; columnOffset < comparableCount; columnOffset++)
            {
                var targetColumnIndex = startColumnIndex + columnOffset;
                if (editableColumns[targetColumnIndex].Header is string tag &&
                    string.Equals(firstRow[columnOffset].Trim(), tag, StringComparison.OrdinalIgnoreCase))
                {
                    headerMatches++;
                }
            }

            if (comparableCount > 0 && headerMatches == comparableCount)
                rowValues.RemoveAt(0);
        }

        if (rowValues.Count == 0)
            return false;

        var pendingUpdates = new Dictionary<(BeamChain Chain, string Tag), double>();
        var touchedRowIndices = new HashSet<int>();

        for (var rowOffset = 0; rowOffset < rowValues.Count; rowOffset++)
        {
            var targetRowIndex = startRowIndex + rowOffset;
            if (targetRowIndex >= dataGrid.Items.Count)
                break;

            if (dataGrid.Items[targetRowIndex] is not UlsResult ulsResult)
                continue;

            var beamChain = ulsResult.Beam?.BeamChain;
            if (beamChain is null)
                continue;

            var rowTouched = false;
            var values = rowValues[rowOffset];
            for (var columnOffset = 0; columnOffset < values.Length; columnOffset++)
            {
                var targetColumnIndex = startColumnIndex + columnOffset;
                if (targetColumnIndex >= editableColumns.Count)
                    break;

                if (!TryParseDouble(values[columnOffset], out var value))
                    continue;

                if (editableColumns[targetColumnIndex].Header is string tag)
                {
                    pendingUpdates[(beamChain, tag)] = value;
                    rowTouched = true;
                }
            }

            if (rowTouched)
                touchedRowIndices.Add(targetRowIndex);
        }

        if (pendingUpdates.Count == 0)
            return false;

        var anyValueApplied = false;
        foreach (var update in pendingUpdates)
        {
            if (TryApplyKValue(update.Key.Chain, update.Key.Tag, update.Value))
                anyValueApplied = true;
        }

        if (anyValueApplied)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);

            var touchedChains = pendingUpdates.Keys.Select(k => k.Chain).ToHashSet();
            RefreshRowsSharingChains(dataGrid, editableColumns, touchedChains);
        }

        return anyValueApplied;
    }

    private static void RefreshRowsSharingChains(DataGrid dataGrid, List<DataGridColumn> editableColumns, HashSet<BeamChain> touchedChains)
    {
        for (var i = 0; i < dataGrid.Items.Count; i++)
        {
            if (dataGrid.Items[i] is not UlsResult ulsResult)
                continue;

            if (ulsResult.Beam?.BeamChain is not BeamChain chain || !touchedChains.Contains(chain))
                continue;

            if (dataGrid.ItemContainerGenerator.ContainerFromIndex(i) is not DataGridRow row)
                continue;

            foreach (var column in editableColumns)
            {
                var cellContent = column.GetCellContent(row);
                if (cellContent is TextBlock textBlock)
                    System.Windows.Data.BindingOperations.GetBindingExpression(textBlock, TextBlock.TextProperty)?.UpdateTarget();
                else if (cellContent is TextBox textBox)
                    System.Windows.Data.BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty)?.UpdateTarget();
            }
        }
    }

    private static bool FillDown(DataGrid dataGrid)
    {
        if (dataGrid.CurrentCell.Item is not UlsResult sourceResult)
            return false;

        var currentColumn = dataGrid.CurrentCell.Column;
        if (currentColumn is null || currentColumn.Header is not string columnHeader || !IsKColumn(columnHeader))
            return false;

        var sourceValue = GetKValue(sourceResult, columnHeader);
        if (sourceValue is null)
            return false;

        var startRowIndex = dataGrid.Items.IndexOf(sourceResult);
        if (startRowIndex < 0)
            return false;

        var selectedRows = dataGrid.SelectedCells
            .Select(cellInfo => cellInfo.Item)
            .Distinct()
            .OfType<UlsResult>()
            .ToList();

        if (selectedRows.Count == 0)
            return false;

        var anyValueApplied = false;
        foreach (var row in selectedRows)
        {
            if (dataGrid.Items.IndexOf(row) >= startRowIndex)
            {
                ApplyKValue(row, columnHeader, sourceValue.Value);
                anyValueApplied = true;
            }
        }

        if (anyValueApplied)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            dataGrid.Items.Refresh();
        }

        return anyValueApplied;
    }

    private static bool FillRight(DataGrid dataGrid)
    {
        if (dataGrid.CurrentCell.Item is not UlsResult sourceResult)
            return false;

        var currentColumn = dataGrid.CurrentCell.Column;
        if (currentColumn is null || currentColumn.Header is not string columnHeader || !IsKColumn(columnHeader))
            return false;

        var sourceValue = GetKValue(sourceResult, columnHeader);
        if (sourceValue is null)
            return false;

        var editableColumns = dataGrid.Columns
            .Where(column => column.Header is string header && IsKColumn(header))
            .OrderBy(column => column.DisplayIndex)
            .ToList();

        var startColumnIndex = editableColumns.FindIndex(column => column == currentColumn);
        if (startColumnIndex < 0)
            return false;

        var anyValueApplied = false;

        var selectedColumns = dataGrid.SelectedCells
            .Select(cellInfo => cellInfo.Column)
            .Distinct()
            .Where(col => col is not null && col.Header is string header && IsKColumn(header))
            .ToList();

        if (selectedColumns.Count == 0)
            return false;

        foreach (var column in selectedColumns)
        {
            if (column is not null && column.Header is string header && dataGrid.CurrentCell.Column is not null &&
                editableColumns.IndexOf(column) >= editableColumns.IndexOf(dataGrid.CurrentCell.Column))
            {
                ApplyKValue(sourceResult, header, sourceValue.Value);
                anyValueApplied = true;
            }
        }

        if (anyValueApplied)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            dataGrid.Items.Refresh();
        }

        return anyValueApplied;
    }

    private static bool IsKColumn(string tag)
    {
        return tag is "K2" or "K1" or "Kz" or "Ke" or "KeB";
    }

    private static double? GetKValue(UlsResult ulsResult, string? tag)
    {
        if (ulsResult == null || string.IsNullOrWhiteSpace(tag))
            return null;

        return tag switch
        {
            "K2" => ulsResult.Beam.BeamChain.K2,
            "K1" => ulsResult.Beam.BeamChain.K1,
            "Kz" => ulsResult.Beam.BeamChain.Kz,
            "Ke" => ulsResult.Beam.BeamChain.KeTop,
            "KeB" => ulsResult.Beam.BeamChain.KeBottom,
            _ => null
        };
    }

    private static void ApplyKValue(UlsResult ulsResult, string? tag, double value)
    {
        if (ulsResult?.Beam?.BeamChain is null)
            return;

        TryApplyKValue(ulsResult.Beam.BeamChain, tag, value);
    }

    private static bool TryApplyKValue(BeamChain beamChain, string? tag, double value)
    {
        if (beamChain is null || string.IsNullOrWhiteSpace(tag))
            return false;

        switch (tag)
        {
            case "K2":
                if (beamChain.K2.Equals(value)) return false;
                beamChain.K2 = value;
                return true;
            case "K1":
                if (beamChain.K1.Equals(value)) return false;
                beamChain.K1 = value;
                return true;
            case "Kz":
                if (beamChain.Kz.Equals(value)) return false;
                beamChain.Kz = value;
                return true;
            case "Ke":
                if (beamChain.KeTop.Equals(value)) return false;
                beamChain.KeTop = value;
                return true;
            case "KeB":
                if (beamChain.KeBottom.Equals(value)) return false;
                beamChain.KeBottom = value;
                return true;
            default:
                return false;
        }
    }

    private static bool TryParseDouble(string value, out double result)
    {
        return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out result)
               || double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result);
    }
}