using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models.Loading;

public partial class WindLoadingModel : ObservableObject
{
    [ObservableProperty]
    public double _windPressure = 1050D; //Default unit is pascals
    [ObservableProperty]
    public double _sharpEdgeFactor = 2D;
    [ObservableProperty]
    public double _circularSectionFactor = 0.8D;
    [ObservableProperty]
    public double _rectangularSectionFactor = 1.8D;
    [ObservableProperty]
    public double _xVectorCoord = 0D;
    [ObservableProperty]
    public double _yVectorCoord = 0D;
    [ObservableProperty]
    public double _zVectorCoord = 0D;

    public void SetVector(double[] windLoadVector)
    {
        XVectorCoord = windLoadVector[0]; 
        YVectorCoord = windLoadVector[1]; 
        ZVectorCoord = windLoadVector[2];
    }
}
