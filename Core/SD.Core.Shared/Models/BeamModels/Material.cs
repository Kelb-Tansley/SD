
using CommunityToolkit.Mvvm.ComponentModel;
using SD.Core.Shared.Extensions;
using System.Collections.ObjectModel;

namespace SD.Core.Shared.Models.BeamModels;
/// <summary>
/// Fy refers to the steel yield strength.
/// Fu refers to the steel ultimate strength.
/// 
/// (Common Note: Element 1 refers to the bottom flange for I or H sections, the flange for T sections, the top and bottom flange for channels and rectangular sections, 
/// the x local axis leg of angles and the shell of circular sections. Element 2 refers to the top flange of I or H sections, the web of channels, 
/// rectangular and T sections, the y local axis leg of angles. Element 3 refers to the web of I or H sections.)
/// </summary>
public partial class Material : ObservableObject
{
    /// <summary>
    /// Steel yield strength of the bottom flange for I or H sections, the flange for T sections, the top and bottom flange for channels and rectangular sections, 
    /// the x local axis leg of angles and the shell of circular sections.
    /// </summary>
    public double FyElement1 { get; set; }
    /// <summary>
    /// Steel yield strength for the top flange of I or H sections, the web of channels, 
    /// rectangular and T sections, the y local axis leg of angles.
    /// </summary>
    public double FyElement2 { get; set; }
    /// <summary>
    /// Steel yield strength for the web of I or H sections
    /// </summary>
    public double FyElement3 { get; set; }
    /// <summary>
    /// Minimum yield strength across all section elements
    /// </summary>
    [ObservableProperty]
    public double minFy;
    /// <summary>
    /// Ultimate strength of steel for element 1 as explained above.
    /// </summary>
    public double FuElement1 { get; set; }
    /// <summary>
    /// Ultimate strength of steel for element 2 as explained above.
    /// </summary>
    public double FuElement2 { get; set; }
    /// <summary>
    /// Ultimate strength of steel for element 3 as explained above.
    /// </summary>
    public double FuElement3 { get; set; }
    /// <summary>
    /// Elasticity of structural steel
    /// </summary>
    public double Es { get; set; }
    /// <summary>
    /// Shear modulus of steel
    /// </summary>
    public double Gs { get; set; }
    /// <summary>
    /// Density of material in kg/m3
    /// </summary>
    public double Density { get; set; }

    [ObservableProperty]
    public ObservableCollection<string>? _availableSteelGrades;

    [ObservableProperty]
    public string? _steelGrade;

    public Material(double fyElement1, double fyElement2, double fyElement3)
    {
        FyElement1 = fyElement1;
        FyElement2 = fyElement2;
        FyElement3 = fyElement3;

        SetMinFy();
    }

    public void SetMinFy()
    {
        MinFy = DoubleExtensions.GetMinimumNonZero(FyElement1, FyElement2, FyElement3);
    }
}