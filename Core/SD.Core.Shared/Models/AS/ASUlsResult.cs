namespace SD.Core.Shared.Models.AS;
public partial class ASUlsResult : UlsResult
{
    public ASUlsResult() => DesignCode = Enum.DesignCode.AS;

    public required ASCapacity Capacity { get; set; }

    public required ASUtilisation Utilization { get; set; }

    public override double? MaxUtilization()
    {
        return Utilization?.MaxUtilization;
    }
}
