using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models.BeamModels;
public abstract partial class Section : SectionProperties
{
    public Section(SectionType sectionType, Material material)
    {
        SectionType = sectionType;
        Material = material;
        SetDefaultRestraints();
    }

    public Material Material { get; private set; }
    public SectionType SectionType { get; private set; }

    public bool CanDesign { get; set; }
    public bool IsPlateGirder { get; set; }
    public bool IsBracedFrame { get; set; } = true;
    /// <summary>
    /// Centre-to-centre distance between transverse web stiffeners
    /// </summary>
    public double WebStiffenerSpacing { get; set; } = double.PositiveInfinity;
    /// <summary>
    /// This factor is the largest reduction on Ane (net area) due to the connection conditions of the beam.
    /// Ane' * NetAreaFactor = Ag (gross area)
    /// </summary>
    public double NetAreaFactor { get; set; } = 0.85;

    public bool IsLateralRestraint { get; set; } = true;

    public bool IsTorsionalRestraint { get; set; }

    public bool IsTopFlangeRestraint { get; set; }

    public bool IsBottomFlangeRestraint { get; set; }

    public string DisplayName { get => GetDisplayName(); }
    public string TypeDisplay { get => GetTypeDisplay(); }
    public double SectionMass { get => GetSectionMass(); }

    private double GetSectionMass()
    {
        return Agr * Material.Density;
    }

    protected abstract string GetTypeDisplay();
    protected abstract string GetDisplayName();

    protected abstract void SetDefaultRestraints();
}