namespace SD.Fem.Strand7.Interfaces;
public interface IStrandResultsService
{
    public double GetStartDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor);
    public double GetEndDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations);

    public double GetMinDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations);
    public double GetMaxDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations);
    public double GetMaxDeflectionBySlope(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations, double slope, double beamLength);
    public double GetMinDeflectionBySlope(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations, double slope, double beamLength);
    public double GetDeflectionDifference(double start, double end);
}
