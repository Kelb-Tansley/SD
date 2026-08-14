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

    private static List<double> GetResultCurveValues(StrandBeamResults result, BeamResultType resultType)
    {
        var values = new List<double>();

        for (int station = 1; station <= result.NumStations; station++)
        {
            var index = (station - 1) * result.NumColumns + (int)resultType;
            values.Add(result.BeamRes[index]);
        }

        return values;
    }

    public static bool IsResultCurveColinear(this List<StrandBeamResults> results,
                                             BeamResultType resultType,
                                             double relativeTolerance = 0.001D,
                                             double absoluteTolerance = 1E-6D)
    {
        foreach (var result in results)
        {
            var values = GetResultCurveValues(result, resultType);

            var firstValue = values[0];
            var lastValue = values[^1];
            var firstPos = result.BeamPos[0];
            var lastPos = result.BeamPos[values.Count - 1];
            var totalDistance = lastPos - firstPos;
            var maxAbsValue = values.Max(Math.Abs);
            var tolerance = Math.Max(absoluteTolerance, relativeTolerance * Math.Max(1D, maxAbsValue));

            for (int i = 1; i < values.Count - 1; i++)
            {
                var deltaX = result.BeamPos[i] - firstPos;
                var expected = firstValue + (lastValue - firstValue) * (deltaX / totalDistance);
                if (Math.Abs(values[i] - expected) > tolerance)
                    return false;
            }
        }
        return true;
    }

    public static bool IsResultCurveSingleSlopeChange(this List<StrandBeamResults> results,
                                                      BeamResultType resultType,
                                                      double relativeTolerance = 0.001D)
    {
        foreach (var result in results)
        {
            var values = GetResultCurveValues(result, resultType);

            // Strand7 result values are not sampled at equally spaced stations along the beam,
            // so the x-step is not constant.
            var deltaX = result.BeamPos[1] - result.BeamPos[0];
            var previousSlope = (values[1] - values[0]) / deltaX;
            var slopeChanges = 0;

            for (int i = 2; i < values.Count; i++)
            {
                deltaX = result.BeamPos[i] - result.BeamPos[i - 1];
                var currentSlope = (values[i] - values[i - 1]) / deltaX;
                var tolerance = Math.Max(relativeTolerance * Math.Abs(previousSlope), relativeTolerance);

                if (Math.Abs(currentSlope - previousSlope) > tolerance)
                {
                    if (deltaX > relativeTolerance)
                    {
                        slopeChanges++;
                        if (slopeChanges > 1)
                            return false;

                        previousSlope = currentSlope;
                    }
                }
            }

        }

        return true;
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
