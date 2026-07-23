using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models.Loading;

public partial class WindLoadingModel : ObservableObject
{
    [ObservableProperty]
    public partial double WindPressure { get; set; } = 1050D;

    [ObservableProperty]
    public partial double SharpEdgeFactor { get; set; } = 2D;

    [ObservableProperty]
    public partial double CircularSectionFactor { get; set; } = 0.8D;

    [ObservableProperty]
    public partial double RectangularSectionFactor { get; set; } = 1.8D;
    [ObservableProperty]
    public partial double XVectorCoord { get; set; } = 0D;
    [ObservableProperty]
    public partial double YVectorCoord { get; set; } = 0D;
    [ObservableProperty]
    public partial double ZVectorCoord { get; set; } = 0D;

    public void SetVector(double[] windLoadVector)
    {
        XVectorCoord = windLoadVector[0]; 
        YVectorCoord = windLoadVector[1]; 
        ZVectorCoord = windLoadVector[2];
    }
}
