using SD.Core.Shared.Enum;
using SD.Core.Shared.Models;

namespace SD.Core.Shared.Contracts;
public interface IDesignModel
{
    public string DesignCode { get; set; }
    public string VerticalAxis { get; set; }
    public SolverType SolverType { get; set; }
    public bool IsDesignLengthCalculated { get; set; }
    public LoadCaseCombination? LoadCaseCombination { get; set; }
    public BeamDesignSettings DesignSettings { get; set; }
}