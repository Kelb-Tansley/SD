using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SD.UI.UltimateLimitState.Behaviors
{
    public class NavigateDataGridContextMenuBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty ShowInAnotherViewCommandProperty = DependencyProperty.Register(
            nameof(ShowInAnotherViewCommand),
            typeof(ICommand),
            typeof(NavigateDataGridContextMenuBehavior),
            new PropertyMetadata(null));

        public ICommand ShowInAnotherViewCommand
        {
            get => (ICommand)GetValue(ShowInAnotherViewCommandProperty);
            set => SetValue(ShowInAnotherViewCommandProperty, value);
        }

        public static readonly DependencyProperty ShowInAnotherViewTextProperty = DependencyProperty.Register(
            nameof(ShowInAnotherViewText),
            typeof(string),
            typeof(NavigateDataGridContextMenuBehavior),
            new PropertyMetadata(null));

        public string ShowInAnotherViewText
        {
            get => (string)GetValue(ShowInAnotherViewTextProperty);
            set => SetValue(ShowInAnotherViewTextProperty, value);
        }

        public static readonly DependencyProperty CanCopyCellsProperty = DependencyProperty.Register(
            nameof(CanCopyCells),
            typeof(bool),
            typeof(NavigateDataGridContextMenuBehavior),
            new PropertyMetadata(true));

        public bool CanCopyCells
        {
            get => (bool)GetValue(CanCopyCellsProperty);
            set => SetValue(CanCopyCellsProperty, value);
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

            if (CanCopyCells)
            {
                var copyMenuItem = new MenuItem { Header = "Copy" };
                copyMenuItem.Click += CopyMenuItem_Click;
                contextMenu.Items.Add(copyMenuItem);
                contextMenu.Items.Add(new Separator());
            }

            var showInUlsMenuItem = new MenuItem { Header = ShowInAnotherViewText };
            showInUlsMenuItem.Click += ShowInUlsMenuItem_Click;

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
            if (item != null && ShowInAnotherViewCommand != null && ShowInAnotherViewCommand.CanExecute(item))
                ShowInAnotherViewCommand.Execute(item);
        }
    }
}
