using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models;
public class BeamDesignSettings : ObservableObject
{
    public double BeamAllignmentAngleTolerance { get; set; } = 2;
    public double BeamRotationAngleTolerance { get; set; } = 1;
    public double BeamRestraintAngleTolerance { get; set; } = 10;
    public int BeamMinStations { get; set; } = 21;
    public bool IncludeSlendernessCheck { get; set; } = true;
    public bool IncludeAllowableStressCheck { get; set; } = true;
}
