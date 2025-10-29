using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;
using SD.Element.Design.Interfaces;

namespace SD.UI.Singletons;
public partial class DesignModel : ObservableObject, IDesignModel
{
    private readonly IConnectionService? _connectionService;

    public DesignModel(IConnectionService connectionService)
    {
        _connectionService = connectionService;
        DesignCode = GetDesignCodeFromUserLoction();
    }

    private string GetDesignCodeFromUserLoction()
    {
        return _connectionService?.GetUserCurrentCountry() == "AU" ? DesignServiceTypes.ASDesign : DesignServiceTypes.SansDesign;
    }

    [ObservableProperty]
    public required string? designCode;
    [ObservableProperty]
    public string verticalAxis = "Y";
    [ObservableProperty]
    public SolverType solverType = SolverType.LSA;
    [ObservableProperty]
    public bool isDesignLengthCalculated = true;
    [ObservableProperty]
    public required LoadCaseCombination? loadCaseCombination;
    [ObservableProperty]
    public BeamDesignSettings designSettings = new();
}