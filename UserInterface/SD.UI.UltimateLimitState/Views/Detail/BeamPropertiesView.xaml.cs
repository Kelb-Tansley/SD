using SD.UI.Controls;
using SD.UI.UltimateLimitState.ViewModels;
using System.Windows.Controls;

namespace SD.UI.UltimateLimitState.Views
{
    /// <summary>
    /// Interaction logic for BeamPropertiesView.xaml
    /// </summary>
    public partial class BeamPropertiesView : UserControl
    {
        public BeamPropertiesView()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == null)
                return;

            var comboBox = (ComboBox)sender;
            if (comboBox == null)
                return;

            var uc = Ancestor.FindAncestor<UserControl>(comboBox);
            if (uc == null || uc.DataContext == null)
                return;

            var dc = (BeamPropertiesViewModel)uc.DataContext;
            dc.SteelGradeChanged(comboBox.SelectedValue?.ToString());
        }
    }
}
