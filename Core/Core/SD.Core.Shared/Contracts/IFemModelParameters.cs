using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using System.Collections.ObjectModel;

namespace SD.Core.Shared.Contracts;
public interface IFemModelParameters
{
    public bool IsInitialized { get; set; }
    public ObservableCollection<Section> BeamProperties { get; set; }
    public ObservableCollection<Beam> Beams { get; set; }
    public UnitFactor UnitFactor { get; set; }
    public ObservableCollection<LoadCaseCombination> LoadCaseCombinations { get; set; }
    public void Clear();
}
