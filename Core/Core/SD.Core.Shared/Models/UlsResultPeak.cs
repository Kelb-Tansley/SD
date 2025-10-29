using SD.Core.Shared.Enum;

namespace SD.Core.Shared.Models;
public class UlsResultPeak
{
    public int BeamId { get; set; }
    public int LoadCaseId { get; set; }
    public double Utilization { get; set; }
    public DesignCode DesignCode { get; set; } = DesignCode.SANS;
}