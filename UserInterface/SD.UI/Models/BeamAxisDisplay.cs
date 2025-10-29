using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Enum;
using System.Collections.ObjectModel;

namespace SD.UI.Models;
public partial class BeamAxisDisplay : ObservableObject
{
    public BeamAxisDisplay(string designCode)
    {
        DesignLengths = GetDefaultLengths(designCode);
    }

    [ObservableProperty]
    public ObservableCollection<BeamAxisDisplayModel> _designLengths;

    [ObservableProperty]
    public BeamAxisDisplayModel? _selectedDesignLength;

    private static ObservableCollection<BeamAxisDisplayModel> GetDefaultLengths(string designCode)
    {
        switch (designCode)
        {
            case DesignServiceTypes.ASDesign:
                return [
                    new() { DisplayName = "Major Axis", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Minor Axis", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Torsional Axis (z)", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Top Bending Axis (e)", BeamAxis = BeamAxis.PrincipalETop, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Bottom Bending Axis (e)", BeamAxis = BeamAxis.PrincipalEBottom, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Torsional Axis", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.BeamLength }
                ];
            default:
                return [
                    new() { DisplayName = "Major Axis (2)", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Minor Axis (1)", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Torsional Axis (z)", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Top Bending Axis (e)", BeamAxis = BeamAxis.PrincipalETop, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "Bottom Bending Axis (e)", BeamAxis = BeamAxis.PrincipalEBottom, ResultType = ResultType.BeamLength },
                    new() { DisplayName = "KL2/r2", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.Slenderness },
                    new() { DisplayName = "KL1/r1", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.Slenderness }
                ];
        }
    }
}

public class BeamAxisDisplayModel
{
    public required string DisplayName { get; set; }
    public required BeamAxis BeamAxis { get; set; }
    public required ResultType ResultType { get; set; }
}