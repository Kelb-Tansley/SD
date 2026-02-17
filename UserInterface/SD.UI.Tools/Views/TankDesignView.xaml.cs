using SD.UI.Tools.ViewModels;
using System.Windows.Controls;

namespace SD.UI.Tools.Views;

/// <summary>
/// Interaction logic for TankDesignView.xaml
/// </summary>
public partial class TankDesignView : UserControl
{
    public TankDesignView()
    {
        InitializeComponent();
    }

    private void FemModelPanel_SizeChanged(object sender, EventArgs e)
    {
        var vm = DataContext as TankDesignViewModel;
        if (vm == null || FemModelPanel == null)
            return;

        vm.UpdateFemModelView(FemModelPanel.Handle);
    }
}
