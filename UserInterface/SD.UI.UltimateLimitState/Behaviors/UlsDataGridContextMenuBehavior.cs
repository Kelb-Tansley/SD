using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SD.UI.UltimateLimitState.Behaviors
{
    public class UlsDataGridContextMenuBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty ShowInUlsViewCommandProperty = DependencyProperty.Register(
            nameof(ShowInUlsViewCommand), typeof(ICommand), typeof(UlsDataGridContextMenuBehavior), new PropertyMetadata(null));

        public ICommand ShowInUlsViewCommand
        {
            get => (ICommand)GetValue(ShowInUlsViewCommandProperty);
            set => SetValue(ShowInUlsViewCommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.ContextMenu != null) return;

            var contextMenu = new ContextMenu();
            var copyMenuItem = new MenuItem { Header = "Copy" };
            copyMenuItem.Click += CopyMenuItem_Click; 
            var showInUlsMenuItem = new MenuItem { Header = "Show in ULS view" };
            showInUlsMenuItem.Click += ShowInUlsMenuItem_Click;

            contextMenu.Items.Add(copyMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(showInUlsMenuItem);

            AssociatedObject.ContextMenu = contextMenu;
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.SelectedCells == null || AssociatedObject.SelectedCells.Count == 0)
                return;

            var columns = AssociatedObject.SelectedCells.Select(c => c.Column).Distinct().OrderBy(c => c.DisplayIndex).ToList();
            var rows = AssociatedObject.SelectedCells.Select(c => c.Item).Distinct().OrderBy(r => AssociatedObject.Items.IndexOf(r)).ToList();

            var rowStrings = new List<string>();
            foreach (var row in rows)
            {
                var colValues = new List<string>();
                foreach (var col in columns)
                {
                    if (AssociatedObject.SelectedCells.Any(c => c.Column == col && c.Item == row) && col.GetCellContent(row) is TextBlock cellContent)
                        colValues.Add(cellContent.Text);
                    else
                        colValues.Add("");
                }
                rowStrings.Add(string.Join("\t", colValues));
            }
            Clipboard.SetText(string.Join("\r\n", rowStrings));
        }

        private void ShowInUlsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = AssociatedObject.SelectedItem ?? AssociatedObject.CurrentItem;
            if (item != null && ShowInUlsViewCommand != null && ShowInUlsViewCommand.CanExecute(item))
                ShowInUlsViewCommand.Execute(item);
        }
    }
}
