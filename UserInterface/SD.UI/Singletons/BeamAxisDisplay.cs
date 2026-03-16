using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SD.Core.Shared.Constants;
using SD.Core.Shared.Contracts;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.UI;
using SD.UI.Events;
using System.Collections.ObjectModel;

namespace SD.UI.Models;

public partial class BeamAxisDisplay : ObservableObject, IBeamAxisDisplay
{
    private readonly IDesignModel _designModel;
    private readonly DesignContourChangedEvent _designContourChangedEvent;
    private readonly DesignCodeChangedEvent _designCodeChangedEvent;

    public BeamAxisDisplay(IEventAggregator eventAggregator,
                           IDesignModel designModel)
    {
        _designModel = designModel ?? throw new ArgumentNullException(nameof(designModel));

        _designContourChangedEvent = eventAggregator.GetEvent<DesignContourChangedEvent>();
        _designCodeChangedEvent = eventAggregator.GetEvent<DesignCodeChangedEvent>();

        _designCodeChangedEvent.Subscribe(OnDesignCodeChanged);
        OnDesignCodeChanged();
        SelectedDesignLength = DesignLengths.FirstOrDefault();
    }

    private void OnDesignCodeChanged()
    {
        DesignLengths = GetDefaultLengths(_designModel.DesignCode);
        KFactors = GetKFactors(_designModel.DesignCode);
        UlsUtilizationTypes = GetUlsUtilizationTypes(_designModel.DesignCode);
        SlendernessOrientations = GetSlendernessOrientations(_designModel.DesignCode);
    }

    [ObservableProperty]
    public ObservableCollection<BeamAxisDisplayModel>? _designLengths;

    [ObservableProperty]
    public ObservableCollection<BeamAxisDisplayModel>? _kFactors;

    [ObservableProperty]
    public ObservableCollection<BeamAxisDisplayModel>? _slendernessOrientations;

    [ObservableProperty]
    public ObservableCollection<BeamAxisDisplayModel>? _ulsUtilizationTypes;


    [ObservableProperty]
    public BeamAxisDisplayModel? _selectedDesignLength;
    [ObservableProperty]
    public BeamAxisDisplayModel? _selectedKFactor;
    [ObservableProperty]
    public BeamAxisDisplayModel? _selectedSlendernessOrientation;
    [ObservableProperty]
    public BeamAxisDisplayModel? _selectedUlsUtilizationType;

    private static ObservableCollection<BeamAxisDisplayModel> GetDefaultLengths(string designCode)
    {
        return designCode switch
        {
            DesignServiceTypes.ASDesign =>
            [
                new() { DisplayName = "Major Axis", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Minor Axis", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Torsional Axis (z)", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Top Bending Axis (e)", BeamAxis = BeamAxis.PrincipalETop, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Bottom Bending Axis (e)", BeamAxis = BeamAxis.PrincipalEBottom, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Torsional Axis", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.BeamLength }
            ],
            _ =>
            [
                new() { DisplayName = "Major Axis (2)", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Minor Axis (1)", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Torsional Axis (z)", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Top Bending Axis (e)", BeamAxis = BeamAxis.PrincipalETop, ResultType = ResultType.BeamLength },
                new() { DisplayName = "Bottom Bending Axis (e)", BeamAxis = BeamAxis.PrincipalEBottom, ResultType = ResultType.BeamLength }
            ],
        };
    }
    private static ObservableCollection<BeamAxisDisplayModel> GetKFactors(string designCode)
    {
        return designCode switch
        {
            DesignServiceTypes.ASDesign =>
            [
                new() { DisplayName = "K2 - Major Axis", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.KFactor },
                new() { DisplayName = "K1 - Minor Axis", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.KFactor },
                new() { DisplayName = "Kz - Torsional Axis (z)", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.KFactor },
                new() { DisplayName = "Ke - Top Bending Axis (e)", BeamAxis = BeamAxis.PrincipalETop, ResultType = ResultType.KFactor },
                new() { DisplayName = "Ke - Bottom Bending Axis (e)", BeamAxis = BeamAxis.PrincipalEBottom, ResultType = ResultType.KFactor },
                new() { DisplayName = "Kt - Torsional Axis", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.KFactor }
            ],
            _ =>
            [
                new() { DisplayName = "K2 - Major Axis", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.KFactor },
                new() { DisplayName = "K1 - Minor Axis", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.KFactor },
                new() { DisplayName = "Kz - Torsional Axis (z)", BeamAxis = BeamAxis.PrincipalZ, ResultType = ResultType.KFactor },
                new() { DisplayName = "Ke - Top Bending Axis (e)", BeamAxis = BeamAxis.PrincipalETop, ResultType = ResultType.KFactor },
                new() { DisplayName = "Ke - Bottom Bending Axis (e)", BeamAxis = BeamAxis.PrincipalEBottom, ResultType = ResultType.KFactor }
            ],
        };
    }
    private static ObservableCollection<BeamAxisDisplayModel> GetUlsUtilizationTypes(string designCode)
    {
        return designCode switch
        {
            DesignServiceTypes.ASDesign =>
            [
                new() { DisplayName = "Peak", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.All },
                new() { DisplayName = "Tu/Tr", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.Tension },
                new() { DisplayName = "Cu/Cr", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.Compression },
                new() { DisplayName = "Mu/Mr(Major)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.BendingMajor },
                new() { DisplayName = "Mu/Mr(Minor)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.BendingMinor },
                new() { DisplayName = "Mu/Mr(1+2)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.BiAxialBending },
                new() { DisplayName = "Vu/Vr(Major)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearMajor },
                new() { DisplayName = "Vu/Vr(Minor)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearMinor },
                new() { DisplayName = "13.8 a)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.CompressionAndBendingSectionStrength },
                new() { DisplayName = "13.8 b)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.CompressionAndBendingMemberStrength },
                new() { DisplayName = "13.8 c)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.CompressionAndBendingBucklingStrength },
                new() { DisplayName = "V+M(Major)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearAndBendingMajor },
                new() { DisplayName = "V+M(Minor)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearAndBendingMinor },
                new() { DisplayName = "T+M(1+2)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.TensionAndBending },
                new() { DisplayName = "Von Mises", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.AllowableStress },
            ],
            _ =>
            [
                new() { DisplayName = "Peak", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType =  SansUtilizationType.All },
                new() { DisplayName = "Tu/Tr", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.Tension },
                new() { DisplayName = "Cu/Cr", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.Compression },
                new() { DisplayName = "Mu/Mr(Major)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.BendingMajor },
                new() { DisplayName = "Mu/Mr(Minor)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.BendingMinor },
                new() { DisplayName = "Mu/Mr(1+2)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.BiAxialBending },
                new() { DisplayName = "Vu/Vr(Major)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearMajor },
                new() { DisplayName = "Vu/Vr(Minor)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearMinor },
                new() { DisplayName = "13.8 a)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.CompressionAndBendingSectionStrength },
                new() { DisplayName = "13.8 b)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.CompressionAndBendingMemberStrength },
                new() { DisplayName = "13.8 c)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.CompressionAndBendingBucklingStrength },
                new() { DisplayName = "V+M(Major)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearAndBendingMajor },
                new() { DisplayName = "V+M(Minor)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.ShearAndBendingMinor },
                new() { DisplayName = "T+M(1+2)", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.TensionAndBending },
                new() { DisplayName = "Von Mises", BeamAxis = BeamAxis.All, ResultType = ResultType.UlsUtilizationType, SansUtilizationType = SansUtilizationType.AllowableStress },
            ],
        };
    }
    private static ObservableCollection<BeamAxisDisplayModel> GetSlendernessOrientations(string designCode)
    {
        return designCode switch
        {
            DesignServiceTypes.ASDesign => [],
            _ =>
            [
                new() { DisplayName = "KL2/r2", BeamAxis = BeamAxis.Principal2, ResultType = ResultType.Slenderness },
                new() { DisplayName = "KL1/r1", BeamAxis = BeamAxis.Principal1, ResultType = ResultType.Slenderness }
            ],
        };
    }


    [RelayCommand]
    private void SelectedDesignLengthChanged()
    {
        if (SelectedDesignLength is null)
            return;

        SelectedKFactor = null;
        SelectedSlendernessOrientation = null;
        SelectedUlsUtilizationType = null;

        _designContourChangedEvent.Publish();
    }

    [RelayCommand]
    private void SelectedKFactorChanged()
    {
        if (SelectedKFactor is null)
            return;

        SelectedDesignLength = null;
        SelectedSlendernessOrientation = null;
        SelectedUlsUtilizationType = null;

        _designContourChangedEvent.Publish();
    }

    [RelayCommand]
    private void SelectedSlendernessOrientationChanged()
    {
        if (SelectedSlendernessOrientation is null)
            return;

        SelectedDesignLength = null;
        SelectedKFactor = null;
        SelectedUlsUtilizationType = null;

        _designContourChangedEvent.Publish();
    }

    [RelayCommand]
    private void SelectedUlsUtilizationTypeChanged()
    {
        if (SelectedUlsUtilizationType is null)
            return;

        SelectedDesignLength = null;
        SelectedKFactor = null;
        SelectedSlendernessOrientation = null;

        _designContourChangedEvent.Publish();
    }
}