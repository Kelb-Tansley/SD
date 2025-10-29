using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models.UI;
public partial class BeamDisplayComponent : ObservableObject
{
    [ObservableProperty]
    public bool shearForceX = false;
    [ObservableProperty]
    public bool shearForceY = true;
    [ObservableProperty]
    public bool bendingMomentX = false;
    [ObservableProperty]
    public bool bendingMomentY = true;
    [ObservableProperty]
    public bool axialForce = true;
    [ObservableProperty]
    public bool torque = false;
}
