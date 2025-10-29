using SD.UI.UltimateLimitState.ViewModels;

namespace SD.UI.UltimateLimitState.Views
{
    /// <summary>
    /// Interaction logic for BeamFemView.xaml
    /// </summary>
    public partial class BeamFemModelView : System.Windows.Controls.UserControl
    {
        public BeamFemModelView()
        {
            InitializeComponent();
        }
        private void FemBeamDesignPanel_SizeChanged(object sender, EventArgs e)
        {
            var vm = DataContext as BeamFemModelViewModel;
            if (vm == null || FemBeamDesignPanel == null)
                return;

            vm.UpdateFemModelView(FemBeamDesignPanel.Handle);
        }
    }
}
