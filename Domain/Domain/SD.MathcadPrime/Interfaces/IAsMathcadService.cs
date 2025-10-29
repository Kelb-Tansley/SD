using SD.Core.Shared.Models.AS;

namespace SD.MathcadPrime.Interfaces;

public interface IAsMathcadService : IMathcadService
{
    public void ExportToMathcadFile(string? templateFile, ASUlsResult sansUlsResult, bool saveFile = false);
    public ASCapacity ReadASMathcadCombinationResults(string mathcadSheet);
}