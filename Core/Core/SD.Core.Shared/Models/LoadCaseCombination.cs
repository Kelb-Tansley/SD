using CommunityToolkit.Mvvm.ComponentModel;

namespace SD.Core.Shared.Models;
public partial class LoadCaseCombination : Identity
{
    [ObservableProperty]
    public bool _include;

    public static bool AreEqual(LoadCaseCombination a, LoadCaseCombination b) => a.Name.Equals(b.Name) && a.Number.Equals(b.Number);

    public static LoadCaseCombination Clone(LoadCaseCombination lcc) => new()
    {
        Include = lcc.Include,
        Name = lcc.Name,
        Number = lcc.Number,
    };
}