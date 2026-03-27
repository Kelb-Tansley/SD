namespace SD.Fem.Strand7.Interfaces;

public interface IContourFileService
{
    Task<string> GenerateDesignableBeamsContourFile(List<Beam> visibleBeams);
    Task<string> GenerateL1ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateL2ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateLzContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateLeTopContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateLeBottomContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateL1R1ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateL2R2ContourFile(List<Beam> beams, double lengthFactor);
    Task<string> GenerateResultsContourFile(List<UlsResultPeak> results);
    Task<string> GenerateSlsResultsContourFile(List<DeflectionResult> results, DeflectionAxis deflectionAxis);
    Task<string> GenerateK1ContourFile(List<Beam> beams);
    Task<string> GenerateK2ContourFile(List<Beam> beams);
    Task<string> GenerateKzContourFile(List<Beam> beams);
    Task<string> GenerateKeTopContourFile(List<Beam> beams);
    Task<string> GenerateKeBottomContourFile(List<Beam> beams);
}