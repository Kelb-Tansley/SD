using SD.UI.Tools.ViewModels;
using System.Windows.Controls;

namespace SD.UI.Tools.Views;

public partial class WindLoadingView : UserControl
{
    public WindLoadingView()
    {
        InitializeComponent();
    }

    private void FemModelPanel_SizeChanged(object sender, EventArgs e)
    {
        var vm = DataContext as WindLoadingViewModel;
        if (vm == null || FemModelPanel == null)
            return;

        vm.UpdateFemModelView(FemModelPanel.Handle);
    }
}
