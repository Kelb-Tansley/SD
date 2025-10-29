using SD.Core.Shared.Models.BeamModels;
using SD.Core.Shared.Models.UI;

namespace SD.UI.Models;
public class BeamChangedEventModel(Beam selectedBeam, BeamDisplayComponent displayComponent)
{
    public Beam SelectedBeam { get; set; } = selectedBeam;
    public BeamDisplayComponent BeamDisplayComponent { get; set; } = displayComponent;
}
