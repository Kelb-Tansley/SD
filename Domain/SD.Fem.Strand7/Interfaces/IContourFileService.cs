namespace SD.Fem.Strand7.Interfaces;
public interface IContourFileService
{
    Task<string> GenerateL1ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateL2ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateLzContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateLeTopContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateLeBottomContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateL1R1ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateL2R2ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateResultsContourFile(List<UlsResultPeak> results);
    Task<string> GenerateSlsResultsContourFile(List<DeflectionResult> results , DeflectionAxis deflectionAxis);
}
