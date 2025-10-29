using SD.Core.Shared.Models.Sans;

namespace SD.MathcadPrime.Interfaces;

public interface ISansMathcadService : IMathcadService
{
    public SansCapacity ReadSansMathcadResults(string mathcadSheet);
    public void ExportToMathcadFile(string? templateFile, SansUlsResult sansUlsResult, bool saveFile = false);
}