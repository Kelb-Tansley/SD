using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Sans.Services;

namespace SD.Tests.Sans.UnitTests;

[TestClass]
public class W1ValuesUnitTests
{
    [TestMethod]
    public void CalculateW1_MajorAxis_EndMomentsDominant_UsesCase1Rule()
    {
        var forces = new BeamForces
        {
            StartMuMajor = 200,
            EndMuMajor = 100,
            MaxMuMajor = 200,
            MinMuMajor = 0,
            StartMuMinor = 80,
            EndMuMinor = 80,
            MaxMuMinor = 80,
            MinMuMinor = 0,
        };

        var sbc = new BendingConstants
        {
            MuMajorQuarter = 50,
            MuMajorHalf = 50,
            MuMajorThreeQuarter = 50,
            MuMinorQuarter = 40,
            MuMinorHalf = 40,
            MuMinorThreeQuarter = 40,
        };

        InvokeCalculateW1Values(forces, sbc);

        Assert.AreEqual(1, sbc.Loadω1Case);
        Assert.AreEqual(0.8D, sbc.ω1Major, 1e-9);
    }

    [TestMethod]
    public void CalculateW1_MajorAxis_InteriorMomentAboveEndMoments_UsesCase2Rule()
    {
        var forces = new BeamForces
        {
            StartMuMajor = 100,
            EndMuMajor = 100,
            MaxMuMajor = 120,
            MinMuMajor = 0,
            StartMuMinor = 80,
            EndMuMinor = 80,
            MaxMuMinor = 80,
            MinMuMinor = 0,
        };

        var sbc = new BendingConstants
        {
            MuMajorQuarter = 120,
            MuMajorHalf = 50,
            MuMajorThreeQuarter = 50,
            MuMinorQuarter = 40,
            MuMinorHalf = 40,
            MuMinorThreeQuarter = 40,
        };

        InvokeCalculateW1Values(forces, sbc);

        Assert.AreEqual(2, sbc.Loadω1Case);
        Assert.AreEqual(1D, sbc.ω1Major, 1e-9);
    }

    [TestMethod]
    public void CalculateW1_MinorAxis_EndMomentsDominant_UsesCase1Rule()
    {
        var forces = new BeamForces
        {
            StartMuMajor = 120,
            EndMuMajor = 120,
            MaxMuMajor = 120,
            MinMuMajor = 0,
            StartMuMinor = 200,
            EndMuMinor = 100,
            MaxMuMinor = 200,
            MinMuMinor = 0,
        };

        var sbc = new BendingConstants
        {
            MuMajorQuarter = 60,
            MuMajorHalf = 60,
            MuMajorThreeQuarter = 60,
            MuMinorQuarter = 50,
            MuMinorHalf = 50,
            MuMinorThreeQuarter = 50,
        };

        InvokeCalculateW1Values(forces, sbc);

        Assert.AreEqual(1, sbc.Loadω1Case);
        Assert.AreEqual(0.8D, sbc.ω1Minor, 1e-9);
    }

    private static void InvokeCalculateW1Values(BeamForces forces, BendingConstants sbc)
    {
        var method = typeof(SansDesignService).GetMethod("Calculateω1Values", BindingFlags.NonPublic | BindingFlags.Static);
        method.Should().NotBeNull();
        method!.Invoke(null, [forces, sbc]);
    }
}
