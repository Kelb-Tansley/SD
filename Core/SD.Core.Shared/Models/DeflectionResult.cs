namespace SD.Core.Shared.Models;
public class DeflectionResult
{
    public int BeamId { get; set; }
    public int LoadCaseId { get; set; }
    public double DeflectionRatio { get; set; } // This is the ratio of the beams span to peak absolute of relative deflection
}
