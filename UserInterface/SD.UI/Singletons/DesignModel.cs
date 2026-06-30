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
    public partial bool IsSaveEnabled { get; set; } = false;

    [ObservableProperty]
    public required partial string? DesignCode { get; set; }

    [ObservableProperty]
    public partial string VerticalAxis { get; set; } = "Y";

    [ObservableProperty]
    public partial SolverType SolverType { get; set; } = SolverType.LSA;

    [ObservableProperty]
    public partial bool IsDesignLengthCalculated { get; set; } = true;

    [ObservableProperty]
    public required partial LoadCaseCombination? LoadCaseCombination { get; set; }

    [ObservableProperty]
    public partial ModelDesignSettings DesignSettings { get; set; } = new();
}