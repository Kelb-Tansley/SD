using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Models.BeamModels;
using System.Collections.ObjectModel;

namespace SD.Core.Shared.Models;
public partial class FemModelParameters : ObservableObject, IFemModelParameters
{
    public bool IsInitialized { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Section> BeamProperties { get; set; } = [];
    [ObservableProperty]
    public partial ObservableCollection<Section> NonDesignableSections { get; set; } = [];

    [ObservableProperty]
    public required partial ObservableCollection<Beam> Beams { get; set; } = [];
    public required UnitFactor UnitFactor { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<LoadCaseCombination> LoadCaseCombinations { get; set; } = [];

    public void Clear()
    {
        IsInitialized = false;
        BeamProperties.Clear();
        NonDesignableSections.Clear();
        Beams.Clear();
        UnitFactor = new UnitFactor();
        LoadCaseCombinations.Clear();
    }
}