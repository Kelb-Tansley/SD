using SD.Core.Strand.Enum;

namespace SD.Fem.Strand7.Extensions;
public static class StrandResultsExtension
{
    public static double MaxStressResult(this List<StrandBeamResults> results)
    {
        //Start at the negative most double value
        var maxResult = double.NegativeInfinity;
        foreach (var result in results)
        {
            foreach (var beamStressRes in result.BeamStressRes)
            {
                if (beamStressRes > maxResult)
                    maxResult = beamStressRes;
            }
        }

        return maxResult;
    }
    public static double MaxResult(this List<StrandBeamResults> results, BeamResultType resultType)
    {
        //Start at the negative most double value
        var maxResult = double.NegativeInfinity;
        foreach (var result in results)
        {
            for (int l = 1; l <= result.NumStations; l++)
            {
                //Instantaneous axial force at station along element
                var beamResult = result.BeamRes[(l - 1) * result.NumColumns + (int)resultType];
                if (beamResult > maxResult)
                    maxResult = beamResult;
            }
        }

        return maxResult;
    }
    public static double MaxResult(this double[] beamRes, int numColumns, int numStations, BeamResultType resultType)
    {
        //Start at the negative most double value
        var maxResult = double.NegativeInfinity;

        for (int l = 1; l <= numStations; l++)
        {
            //Instantaneous axial force at station along element
            var result = beamRes[(l - 1) * numColumns + (int)resultType];
            if (result > maxResult)
                maxResult = result;
        }

        return maxResult;
    }
    public static double MinResult(this List<StrandBeamResults> results, BeamResultType resultType)
    {
        //Start at the positive most double value
        var minResult = double.PositiveInfinity;
        foreach (var result in results)
        {
            for (int l = 1; l <= result.NumStations; l++)
            {
                //Instantaneous axial force at station along element
                var beamResult = result.BeamRes[(l - 1) * result.NumColumns + (int)resultType];
                if (beamResult < minResult)
                    minResult = beamResult;
            }
        }

        return minResult;
    }
    public static double MinResult(this double[] beamRes, int numColumns, int numStations, BeamResultType resultType)
    {
        //Start at the positive most double value
        var minResult = double.PositiveInfinity;

        for (int l = 1; l <= numStations; l++)
        {
            //Instantaneous axial force at station along element
            var result = beamRes[(l - 1) * numColumns + (int)resultType];
            if (result < minResult)
                minResult = result;
        }

        return minResult;
    }
    public static double StartResult(this List<StrandBeamResults> results, BeamResultType resultType)
    {
        return results.First().BeamRes[(int)resultType];
    }
    public static double StartResult(this double[] beamRes, BeamResultType resultType)
    {
        return beamRes[(int)resultType];
    }
    public static double EndResult(this double[] beamRes, int numColumns, int numStations, BeamResultType resultType)
    {
        return beamRes[(numStations - 1) * numColumns + (int)resultType];
    }
    public static double EndResult(this List<StrandBeamResults> results, BeamResultType resultType)
    {
        var lastResult = results.Last();
        return lastResult.BeamRes[(lastResult.NumStations - 1) * lastResult.NumColumns + (int)resultType];
    }
}
