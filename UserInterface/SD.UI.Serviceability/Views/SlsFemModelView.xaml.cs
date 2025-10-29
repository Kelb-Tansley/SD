using SD.UI.Serviceability.ViewModels;

namespace SD.UI.Serviceability.Views
{
    /// <summary>
    /// Interaction logic for SlsFemModelView.xaml
    /// </summary>
    public partial class SlsFemModelView : System.Windows.Controls.UserControl
    {
        public SlsFemModelView()
        {
            InitializeComponent();
        }

        private void SlsFemModelPanel_BindingContextChanged(object sender, EventArgs e)
        {

        }

        private void SlsFemModelPanel_SizeChanged(object sender, EventArgs e)
        {
            var vm = DataContext as SlsFemModelViewModel;
            if (vm == null || SlsFemModelPanel == null)
                return;

            vm.UpdateFemModelView(SlsFemModelPanel.Handle);
        }
    }
}
