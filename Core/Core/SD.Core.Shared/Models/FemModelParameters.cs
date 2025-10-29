using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using System.Collections.ObjectModel;

namespace SD.Core.Shared.Models;
public partial class FemModelParameters : ObservableObject, IFemModelParameters
{
    public bool IsInitialized { get; set; }

    [ObservableProperty]
    public ObservableCollection<Section> beamProperties = [];

    [ObservableProperty]
    public required ObservableCollection<Beam> beams = [];
    public required UnitFactor UnitFactor { get; set; }

    [ObservableProperty]
    public ObservableCollection<LoadCaseCombination> loadCaseCombinations = [];

    public void Clear()
    {
        IsInitialized = false;
        BeamProperties.Clear();
        Beams.Clear();
        UnitFactor = new UnitFactor();
        LoadCaseCombinations.Clear();
    }
}