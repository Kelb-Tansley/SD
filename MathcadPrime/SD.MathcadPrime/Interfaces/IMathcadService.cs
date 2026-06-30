using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;

namespace SD.MathcadPrime.Interfaces;
public interface IMathcadService
{
    /// <summary>
    /// Closes this "Mathcad Prime" application if possible
    /// </summary>
    public void CloseMathcad();
    public void SaveWorksheet();
    public SectionCapacity ReadMathcadTensionResults(string mathcadSheet);
    public SectionCapacity ReadMathcadShearResults(string mathcadSheet);
    public SectionCapacity ReadMathcadCompressionResults(string mathcadSheet);
    public SectionCapacity ReadMathcadBendingResults(string mathcadSheet);
    public string GetMathCadFileName(SectionType sectionType);
}
