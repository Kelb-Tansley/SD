using SD.Core.Shared.Enum;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Core.Shared.Models;
public abstract class UlsResult : IEquatable<UlsResult>
{
    public required Beam Beam { get; set; }

    public required BeamForces Forces { get; set; }

    public required UlsLoads Loads { get; set; }

    public int LoadCaseNumber
    {
        get;
        private set;
    }

    public DesignType DesignType { get; set; }

    public DesignCode DesignCode { get; set; }

    public bool BracedState { get; set; }

    public void SetLoadCaseNumber(int loadCaseNumber)
    {
        LoadCaseNumber = loadCaseNumber;
    }

    public int BeamId()
    {
        return Beam.Number;
    }

    public abstract double? MaxUtilization();

    public override bool Equals(object? obj)
    {
        return Equals(obj as UlsResult);
    }

    public bool Equals(UlsResult? other)
    {
        return other is not null && BeamId() == other.Beam.Number && LoadCaseNumber == other.LoadCaseNumber;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Beam, Forces, Loads, LoadCaseNumber, DesignType, DesignCode, BracedState);
    }

    public static bool operator ==(UlsResult? left, UlsResult? right)
    {
        return EqualityComparer<UlsResult>.Default.Equals(left, right);
    }

    public static bool operator !=(UlsResult? left, UlsResult? right)
    {
        return !(left == right);
    }
}