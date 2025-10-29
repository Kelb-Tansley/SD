namespace SD.Fem.Strand7.Services;
public class StrandResultsService : IStrandResultsService
{
    private readonly int _numDeflectionColumns = 6;
    public double GetStartDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor)
    {
        return beamResults[GetDeflectionAxisValue(deflectionAxis)] * unitFactor.Length;
    }
    public double GetEndDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations)
    {
        return beamResults[(numStations - 1) * _numDeflectionColumns + GetDeflectionAxisValue(deflectionAxis)] * unitFactor.Length;
    }
    public double GetMinDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations)
    {
        var deflectionAxisVal = GetDeflectionAxisValue(deflectionAxis);

        // Set the min deflection to the start deflection to accomodate positive and negative relative deflection
        var deflection = GetStartDeflection(beamResults, deflectionAxis, unitFactor);

        for (int l = 1; l <= numStations; l++)
            deflection = Math.Min(beamResults[(l - 1) * _numDeflectionColumns + deflectionAxisVal] * unitFactor.Length, deflection);

        return deflection;
    }
    public double GetMaxDeflection(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations)
    {
        var deflectionAxisVal = GetDeflectionAxisValue(deflectionAxis);

        // Set the max deflection to the start deflection to accomodate positive and negative relative deflection
        var deflection = GetStartDeflection(beamResults, deflectionAxis, unitFactor);

        for (int l = 1; l <= numStations; l++)
            deflection = Math.Max(beamResults[(l - 1) * _numDeflectionColumns + deflectionAxisVal] * unitFactor.Length, deflection);

        return deflection;
    }

    public double GetMaxDeflectionBySlope(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations, double slope, double beamLength)
    {
        var deflectionAxisVal = GetDeflectionAxisValue(deflectionAxis);

        // Set the max deflection to the start deflection to accomodate positive and negative relative deflection
        var start = GetStartDeflection(beamResults, deflectionAxis, unitFactor);
        var deflection = 0D;

        for (int l = 1; l <= numStations; l++)
        {
            var factor = ((double)l - 1) / numStations;
            var slopeDeflection = slope * factor * beamLength + start;
            var relativeDeflection = GetDeflectionDifference(beamResults[(l - 1) * _numDeflectionColumns + deflectionAxisVal] * unitFactor.Length, slopeDeflection);

            deflection = Math.Max(relativeDeflection, deflection);
        }

        return deflection;
    }

    public double GetMinDeflectionBySlope(double[] beamResults, DeflectionAxis deflectionAxis, UnitFactor unitFactor, int numStations, double slope, double beamLength)
    {
        var deflectionAxisVal = GetDeflectionAxisValue(deflectionAxis);

        // Set the max deflection to the start deflection to accomodate positive and negative relative deflection
        var start = GetStartDeflection(beamResults, deflectionAxis, unitFactor);
        var deflection = 0D;

        for (int l = 1; l <= numStations; l++)
        {
            var factor = ((double)l - 1) / numStations;
            var slopeDeflection = slope * factor * beamLength + start;
            var relativeDeflection = GetDeflectionDifference(beamResults[(l - 1) * _numDeflectionColumns + deflectionAxisVal] * unitFactor.Length, slopeDeflection);

            deflection = Math.Min(relativeDeflection, deflection);
        }

        return deflection;
    }

    private static int GetDeflectionAxisValue(DeflectionAxis deflectionAxis)
    {
        return deflectionAxis switch
        {
            DeflectionAxis.X => St7.ipNodeResFileDX,
            DeflectionAxis.Y => St7.ipNodeResFileDY,
            DeflectionAxis.Z => St7.ipNodeResFileDZ,
            _ => throw new NotImplementedException()
        };
    }

    public double GetDeflectionDifference(double start, double end)
    {
        if ((start < 0 && end < 0) || (start >= 0 && end >= 0))
            return Math.Abs(start - end);
        else if ((start >= 0 && end < 0) || (start < 0 && end >= 0))
            return Math.Abs(start) + Math.Abs(end);
        else
            throw new NotImplementedException();
    }
}
