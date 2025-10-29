using SD.UI.UltimateLimitState.ViewModels;

namespace SD.UI.UltimateLimitState.Views;

/// <summary>
/// Interaction logic for FemModelView.xaml
/// </summary>
public partial class FemModelView : System.Windows.Controls.UserControl
{
    public FemModelView()
    {
        InitializeComponent();
    }

    private void FemModelPanel_SizeChanged(object sender, EventArgs e)
    {
        var vm = DataContext as FemModelViewModel;
        if (vm == null || FemModelPanel == null)
            return;

        vm.UpdateFemModelView(FemModelPanel.Handle);
    }
}
