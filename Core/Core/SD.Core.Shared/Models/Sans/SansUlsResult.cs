using SD.Core.Shared.Models.BeamModels;

namespace SD.Core.Shared.Models.Sans;
public partial class SansUlsResult : UlsResult
{
    public SansUlsResult() => DesignCode = Enum.DesignCode.SANS;

    public required SansCapacity Capacity { get; set; }

    public required SansUtilization Utilization { get; set; }

    public SectionClassification? FlexuralClass { get; set; }

    public SectionClassification? AxialClass { get; set; }

    public override double? MaxUtilization()
    {
        return Utilization?.MaxUtilization;
    }
}
