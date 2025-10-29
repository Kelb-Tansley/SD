using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;

namespace SD.UI.Singletons;
public partial class ASDesignModel : ObservableObject, IDesignModel
{
    [ObservableProperty]
    public string designCode = DesignServiceTypes.ASDesign;
    [ObservableProperty]
    public string verticalAxis = "Y";
    [ObservableProperty]
    public SolverType solverType = SolverType.LSA;
    [ObservableProperty]
    public bool isBracedFrame = true;
    [ObservableProperty]
    public bool isDesignLengthCalculated = true;
    [ObservableProperty]
    public required LoadCaseCombination? loadCaseCombination;
    [ObservableProperty]
    public BeamDesignSettings designSettings = new();
}