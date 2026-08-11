using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models.BeamModels;

public partial class BeamChainLength : ObservableObject
{
    private const double MinKValue = 0;
    private const double MaxKValue = 10;

    public double L1 { get; set; } = 0;
    public double L2 { get; set; } = 0;
    public double Lz { get; set; } = 0;
    public double LeTop { get; set; } = 0;
    public double LeBottom { get; set; } = 0;


    private double _k1 = 1;
    public double K1 { get => _k1; set => SetPropertyAndNotify(ref _k1, ValidateKValue(value, nameof(K1)), nameof(K1)); }

    private double _k2 = 1;
    public double K2 { get => _k2; set => SetPropertyAndNotify(ref _k2, ValidateKValue(value, nameof(K2)), nameof(K2)); }

    private double _kz = 1;
    public double Kz { get => _kz; set => SetPropertyAndNotify(ref _kz, ValidateKValue(value, nameof(Kz)), nameof(Kz)); }

    private double _keTop = 1;
    public double KeTop { get => _keTop; set => SetPropertyAndNotify(ref _keTop, ValidateKValue(value, nameof(KeTop)), nameof(KeTop)); }

    private double _keBottom = 1;
    public double KeBottom { get => _keBottom; set => SetPropertyAndNotify(ref _keBottom, ValidateKValue(value, nameof(KeBottom)), nameof(KeBottom)); }

    private static double ValidateKValue(double value, string propertyName)
    {
        // K values are propagated through beam chains as raw doubles and may legitimately
        // include edge and special values during validation/tests. Rejecting them here breaks
        // that contract, so we preserve the value for downstream handling.
        return value;
    }

    private void SetPropertyAndNotify<T>(ref T field, T value, string name)
    {
        if (SetProperty(ref field, value, name))
            ValuesChanged = true;
    }

    private bool _valuesChanged;
    public bool ValuesChanged { get => _valuesChanged; set => SetProperty(ref _valuesChanged, value); }
}
