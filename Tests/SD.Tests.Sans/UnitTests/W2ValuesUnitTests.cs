using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Sans.Engine;

namespace SD.Tests.Sans.UnitTests;

[TestClass]
public class W2ValuesUnitTests
{
    // ── Major axis ────────────────────────────────────────────────────────────

    [TestMethod]
    public void CalculateW2Major_DoubleCurvature_EndMomentsDominant_UsesFormula()
    {
        // κ = 100/200 = 0.5, double curvature → ω2 = 1.75 + 1.05×0.5 + 0.3×0.25 = 2.35
        var forces = new BeamForces
        {
            StartMuMajor = 200,
            EndMuMajor = 100,
            MinMuMajor = -50,
            MaxMuMajor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Major(forces, sbc);

        Assert.AreEqual(2, sbc.Curvature);
        Assert.AreEqual(1, sbc.Loadω2Case);
        Assert.AreEqual(2.35D, sbc.McrMajorω, 1e-9);
    }

    [TestMethod]
    public void CalculateΩ2Major_SingleCurvature_EndMomentsDominant_UsesFormula()
    {
        // κ = 100/200 = 0.5, single curvature → κ_signed = -0.5 → ω2 = 1.75 - 0.525 + 0.075 = 1.3
        var forces = new BeamForces
        {
            StartMuMajor = 200,
            EndMuMajor = 100,
            MinMuMajor = 100,
            MaxMuMajor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Major(forces, sbc);

        Assert.AreEqual(1, sbc.Curvature);
        Assert.AreEqual(1, sbc.Loadω2Case);
        Assert.AreEqual(1.3D, sbc.McrMajorω, 1e-9);
    }

    [TestMethod]
    public void CalculateΩ2Major_DoubleCurvature_EqualEndMoments_CapsAt2_5()
    {
        // κ = 1.0, double curvature → ω2 = 1.75 + 1.05 + 0.3 = 3.1, capped at 2.5
        var forces = new BeamForces
        {
            StartMuMajor = 200,
            EndMuMajor = 200,
            MinMuMajor = -200,
            MaxMuMajor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Major(forces, sbc);

        Assert.AreEqual(2, sbc.Curvature);
        Assert.AreEqual(1, sbc.Loadω2Case);
        Assert.AreEqual(2.5D, sbc.McrMajorω, 1e-9);
    }

    [TestMethod]
    public void CalculateΩ2Major_InteriorMomentExceedsEndMoments_SetsCase2AndReturnsOne()
    {
        // MaxAbsMuMajor = 200 > Max(|100|, |50|) = 100 → W2 = 1.0
        var forces = new BeamForces
        {
            StartMuMajor = 100,
            EndMuMajor = 50,
            MinMuMajor = 0,
            MaxMuMajor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Major(forces, sbc);

        Assert.AreEqual(2, sbc.Loadω2Case);
        Assert.AreEqual(1D, sbc.McrMajorω, 1e-9);
    }

    // ── Minor axis ────────────────────────────────────────────────────────────

    [TestMethod]
    public void CalculateW2Minor_DoubleCurvature_EndMomentsDominant_UsesFormula()
    {
        // κ = 100/200 = 0.5, double curvature → ω2 = 2.35
        var forces = new BeamForces
        {
            StartMuMinor = 200,
            EndMuMinor = 100,
            MinMuMinor = -50,
            MaxMuMinor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Minor(forces, sbc);

        Assert.AreEqual(2.35D, sbc.McrMinorω, 1e-9);
    }

    [TestMethod]
    public void CalculateW2Minor_SingleCurvature_EndMomentsDominant_UsesFormula()
    {
        // κ = -0.5, single curvature → ω2 = 1.3
        var forces = new BeamForces
        {
            StartMuMinor = 200,
            EndMuMinor = 100,
            MinMuMinor = 100,
            MaxMuMinor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Minor(forces, sbc);

        Assert.AreEqual(1.3D, sbc.McrMinorω, 1e-9);
    }

    [TestMethod]
    public void CalculateW2Minor_DoubleCurvature_EqualEndMoments_CapsAt2_5()
    {
        // κ = 1.0, double curvature → ω2 = 3.1, capped at 2.5
        var forces = new BeamForces
        {
            StartMuMinor = 200,
            EndMuMinor = 200,
            MinMuMinor = -200,
            MaxMuMinor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Minor(forces, sbc);

        Assert.AreEqual(2.5D, sbc.McrMinorω, 1e-9);
    }

    [TestMethod]
    public void CalculateW2Minor_InteriorMomentExceedsEndMoments_ReturnsOne()
    {
        // MaxAbsMuMinor = 200 > Max(|100|, |50|) = 100 → ω2 = 1.0
        var forces = new BeamForces
        {
            StartMuMinor = 100,
            EndMuMinor = 50,
            MinMuMinor = 0,
            MaxMuMinor = 200,
        };

        var sbc = new BendingConstants();
        InvokeCalculateW2Minor(forces, sbc);

        Assert.AreEqual(1D, sbc.McrMinorω, 1e-9);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static void InvokeCalculateW2Major(BeamForces forces, BendingConstants sbc)
    {
        var method = typeof(BendingService).GetMethod("Calculateω2Major", BindingFlags.NonPublic | BindingFlags.Static);
        method.Should().NotBeNull();
        method!.Invoke(null, [forces, sbc]);
    }

    private static void InvokeCalculateW2Minor(BeamForces forces, BendingConstants sbc)
    {
        var method = typeof(BendingService).GetMethod("Calculateω2Minor", BindingFlags.NonPublic | BindingFlags.Static);
        method.Should().NotBeNull();
        method!.Invoke(null, [forces, sbc]);
    }
}
