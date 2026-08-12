using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models.BeamModels;
using SD.Core.Shared.Models.BeamModels.Sections;
using SD.Core.Strand.Enum;
using SD.Core.Strand.Models;
using SD.Fem.Strand7.Extensions;

namespace SD.Tests.Strand7.UnitTests;

[TestClass]
public class StrandResultsExtensionUnitTests
{
    [TestMethod]
    public void IsResultCurveColinear_ReturnsTrue_ForPerfectLinearCurve()
    {
        var moments = BuildLinearValues(100, 10D, 110D);
        var results = new List<StrandBeamResults>
        {
            BuildStrandResult(moments, BeamResultType.BendingMomentMinor, 1)
        };

        var isColinear = results.IsResultCurveColinear(BeamResultType.BendingMomentMinor, relativeTolerance: 1E-4D);

        Assert.IsTrue(isColinear);
    }

    [TestMethod]
    public void IsResultCurveColinear_ReturnsTrue_ForSmallNumericalNoiseWithinTolerance()
    {
        var moments = BuildLinearValues(100, -50D, 50D);
        moments[35] += 0.0025D;
        moments[66] -= 0.003D;

        var results = new List<StrandBeamResults>
        {
            BuildStrandResult(moments, BeamResultType.BendingMomentMajor, 1)
        };

        var isColinear = results.IsResultCurveColinear(BeamResultType.BendingMomentMajor, relativeTolerance: 1E-4D);

        Assert.IsTrue(isColinear);
    }

    [TestMethod]
    public void IsResultCurveColinear_ReturnsFalse_WhenCurveHasNonLinearPoint()
    {
        var moments = BuildLinearValues(100, 0D, 100D);
        moments[50] += 0.2D;

        var results = new List<StrandBeamResults>
        {
            BuildStrandResult(moments, BeamResultType.BendingMomentMajor, 1)
        };

        var isColinear = results.IsResultCurveColinear(BeamResultType.BendingMomentMajor, relativeTolerance: 1E-4D);

        Assert.IsFalse(isColinear);
    }

    [TestMethod]
    public void IsResultCurveColinear_ReturnsTrue_AcrossMultipleResultSegments()
    {
        var firstHalf = BuildLinearValues(50, 0D, 49D);
        var secondHalf = BuildLinearValues(50, 50D, 99D);

        var results = new List<StrandBeamResults>
        {
            BuildStrandResult(firstHalf, BeamResultType.BendingMomentMinor, 1),
            BuildStrandResult(secondHalf, BeamResultType.BendingMomentMinor, 2)
        };

        var isColinear = results.IsResultCurveColinear(BeamResultType.BendingMomentMinor, relativeTolerance: 1E-4D);

        Assert.IsTrue(isColinear);
    }

    [TestMethod]
    public void IsResultCurveSingleSlopeChange_ReturnsFalse_WhenSlopeChangeToleranceMustUpdateWithPreviousSlope()
    {
        var values = new[]
        {
            1000.0D,
            1000.1D,
            1000.3D,
            1000.6D
        };

        var results = new List<StrandBeamResults>
        {
            BuildStrandResult(values, BeamResultType.BendingMomentMinor, 1)
        };

        var isSingleSlopeChange = results.IsResultCurveSingleSlopeChange(BeamResultType.BendingMomentMinor, relativeTolerance: 1E-4D);

        Assert.IsFalse(isSingleSlopeChange);
    }

    private static StrandBeamResults BuildStrandResult(double[] values, BeamResultType resultType, int beamNumber)
    {
        const int numColumns = 6;
        var beamRes = new double[values.Length * numColumns];
        for (int i = 0; i < values.Length; i++)
        {
            var index = i * numColumns + (int)resultType;
            beamRes[index] = values[i];
        }

        return new StrandBeamResults
        {
            Beam = new Beam
            {
                Number = beamNumber,
                Section = new RectangularSection(200D, 200D, 10D, 10D, GetMaterial())
            },
            LoadCaseId = 1,
            NumStations = values.Length,
            NumColumns = numColumns,
            BeamRes = beamRes,
            BeamPos = BuildLinearValues(values.Length, 0D, 10D),
            BeamStressRes = Array.Empty<double>(),
            BeamQuarters = Array.Empty<double>()
        };
    }

    private static double[] BuildLinearValues(int points, double start, double end)
    {
        if (points < 2)
            return [start];

        var values = new double[points];
        for (int i = 0; i < points; i++)
        {
            values[i] = start + (end - start) * i / (points - 1D);
        }

        return values;
    }

    private static Material GetMaterial()
    {
        return new Material(350D, 350D, 350D)
        {
            Es = 200000D,
            Gs = 77000D
        };
    }
}
