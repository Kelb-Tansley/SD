using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models.BeamModels;

public partial class BeamChainLength : ObservableObject
{
    public double L1 { get; set; } = 0;
    public double L2 { get; set; } = 0;
    public double Lz { get; set; } = 0;
    public double LeTop { get; set; } = 0;
    public double LeBottom { get; set; } = 0;


    private double _k1 = 1;
    public double K1 { get => _k1; set => SetPropertyAndNotify(ref _k1, value, nameof(K1)); }

    private double _k2 = 1;
    public double K2 { get => _k2; set => SetPropertyAndNotify(ref _k2, value, nameof(K2)); }

    private double _kz = 1;
    public double Kz { get => _kz; set => SetPropertyAndNotify(ref _kz, value, nameof(Kz)); }

    private double _keTop = 1;
    public double KeTop { get => _keTop; set => SetPropertyAndNotify(ref _keTop, value, nameof(KeTop)); }

    private double _keBottom = 1;
    public double KeBottom { get => _keBottom; set => SetPropertyAndNotify(ref _keBottom, value, nameof(KeBottom)); }

    private void SetPropertyAndNotify<T>(ref T field, T value, string name)
    {
        if (SetProperty(ref field, value, name))
            ValuesChanged = true;
    }

    private bool _valuesChanged;
    public bool ValuesChanged { get => _valuesChanged; set => SetProperty(ref _valuesChanged, value); }
}
