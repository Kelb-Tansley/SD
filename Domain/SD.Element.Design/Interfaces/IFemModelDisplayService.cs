using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using SD.Core.Shared.Models.UI;
using SD.Core.Strand.Models;

namespace SD.Element.Design.Interfaces;
public interface IFemModelDisplayService
{
    public Task DisplayDesignResults(int modelId, string fileName, nint handle, IEnumerable<UlsResultPeak> results);
    public Task DisplayDesignLengths(int modelId, string fileName, nint handle, BeamAxis beamAxisEnum);
    public Task DisplayDesignSlenderness(int modelId, string fileName, nint handle, BeamAxis beamAxisEnum);
    public void UpdateFemModel(int modelId, nint handle);
    public void UpdateBeamFemModel(int modelId, nint handle, int loadCase, ref int childId, bool isInitialized, ZoomLevel zoomLevel, Beam beam, BeamDisplayComponent beamDisplayComponent);
    public bool SetSelectedBeams(int modelId);
    public StrandResultFile OpenFemResultsFile(int modelId, string fileName);
    public void CloseFemResultsFile(int modelId);
    public Result LoadFemModelProperties(int modelId, DesignCode designCode, string fileName, bool closeFirst = false);
    public bool OpenFemFile(int modelId, string fileName, bool closeFirst = true);
    public IEnumerable<Beam> GetDisplayedByGroupBeams(int modelId, IEnumerable<Beam> beams);
    public void ClearFemDisplayModel(int modelId);
    public void ReloadFemDisplayModel(int modelId, string fileName, bool closeFirst = true);
    public Task DisplayDeflectionContours(int modelId, nint handle, double minDeflectionRatio, DeflectionAxis deflectionAxis, IEnumerable<DeflectionResult>? results);
}