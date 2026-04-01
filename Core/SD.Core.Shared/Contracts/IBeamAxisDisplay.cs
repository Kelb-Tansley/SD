using SD.Core.Shared.Models.UI;

namespace SD.Core.Shared.Contracts;

public interface IBeamAxisDisplay
{
    public BeamAxisDisplayModel? SelectedDesignableBeam { get; set; }
    public BeamAxisDisplayModel? SelectedDesignLength { get; set; }
    public BeamAxisDisplayModel? SelectedKFactor { get; set; }
    public BeamAxisDisplayModel? SelectedSlendernessOrientation { get; set; }
    public BeamAxisDisplayModel? SelectedUlsUtilizationType { get; set; }
}