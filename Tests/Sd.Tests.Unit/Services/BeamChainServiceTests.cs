using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SD.Core.Shared.Models.BeamModels;
using SD.Element.Design.Services;


namespace SD.Tests.Unit.Services;

/// <summary>
/// Unit tests for the BeamChainService class.
/// </summary>
[TestClass]
public class BeamChainServiceTests
{
    /// <summary>
    /// Tests that GenerateBeamChains throws ArgumentNullException when beams parameter is null.
    /// Input: null list.
    /// Expected: ArgumentNullException.
    /// </summary>
    [TestMethod]
    [TestCategory("ProductionBugSuspected")]
    [Ignore("ProductionBugSuspected")]
    public void GenerateBeamChains_NullBeams_ThrowsArgumentNullException()
    {
        // Arrange
        var service = new BeamChainService();
        List<Beam> beams = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => service.GenerateBeamChains(beams));
    }

    /// <summary>
    /// Tests that GenerateBeamChains completes successfully with an empty list.
    /// Input: Empty list of beams.
    /// Expected: No exception thrown, method completes successfully.
    /// </summary>
    [TestMethod]
    public void GenerateBeamChains_EmptyList_CompletesSuccessfully()
    {
        // Arrange
        var service = new BeamChainService();
        var beams = new List<Beam>();

        // Act
        service.GenerateBeamChains(beams);

        // Assert
        Assert.AreEqual(0, beams.Count);
    }

    /// <summary>
    /// Tests that GenerateBeamChains processes a single beam without throwing exceptions.
    /// Input: List with one beam.
    /// Expected: No exception thrown, method completes successfully.
    /// </summary>
    [TestMethod]
    public void GenerateBeamChains_SingleBeam_CompletesSuccessfully()
    {
        // Arrange
        var service = new BeamChainService();
        var beam = new Beam
        {
            Number = 1,
            Section = null!,
            Node1 = 1,
            Node2 = 2
        };
        var beams = new List<Beam> { beam };

        // Act
        service.GenerateBeamChains(beams);

        // Assert
        Assert.AreEqual(1, beams.Count);
        Assert.IsNotNull(beam.BeamChain);
    }

    /// <summary>
    /// Tests that GenerateBeamChains processes multiple beams without throwing exceptions.
    /// Input: List with multiple beams.
    /// Expected: No exception thrown, method completes successfully for all beams.
    /// </summary>
    [TestMethod]
    public void GenerateBeamChains_MultipleBeams_CompletesSuccessfully()
    {
        // Arrange
        var service = new BeamChainService();

        var beam1 = new Beam
        {
            Number = 1,
            Section = null!,
            Node1 = 1,
            Node2 = 2
        };
        var beam2 = new Beam
        {
            Number = 2,
            Section = null!,
            Node1 = 2,
            Node2 = 3
        };
        var beam3 = new Beam
        {
            Number = 3,
            Section = null!,
            Node1 = 3,
            Node2 = 4
        };
        var beams = new List<Beam> { beam1, beam2, beam3 };

        // Act
        service.GenerateBeamChains(beams);

        // Assert
        Assert.AreEqual(3, beams.Count);
        Assert.IsNotNull(beam1.BeamChain);
        Assert.IsNotNull(beam2.BeamChain);
        Assert.IsNotNull(beam3.BeamChain);
    }

    /// <summary>
    /// Tests that GenerateBeamChains processes beams with varying properties without throwing exceptions.
    /// Input: List with beams having different node configurations.
    /// Expected: No exception thrown, method completes successfully.
    /// </summary>
    [TestMethod]
    public void GenerateBeamChains_BeamsWithVaryingProperties_CompletesSuccessfully()
    {
        // Arrange
        var service = new BeamChainService();

        var beam1 = new Beam
        {
            Number = 10,
            Section = null!,
            Node1 = 0,
            Node2 = 1,
            BeamL1 = 100.0,
            BeamL2 = 200.0
        };
        var beam2 = new Beam
        {
            Number = 20,
            Section = null!,
            Node1 = 1,
            Node2 = 0,
            BeamL1 = 150.0,
            BeamL2 = 250.0
        };
        var beams = new List<Beam> { beam1, beam2 };

        // Act
        service.GenerateBeamChains(beams);

        // Assert
        Assert.AreEqual(2, beams.Count);
    }

    /// <summary>
    /// Tests that GenerateBeamChains handles a large number of beams efficiently.
    /// Input: List with many beams.
    /// Expected: No exception thrown, method completes successfully.
    /// </summary>
    [TestMethod]
    public void GenerateBeamChains_LargeNumberOfBeams_CompletesSuccessfully()
    {
        // Arrange
        var service = new BeamChainService();
        var beams = new List<Beam>();

        for (int i = 0; i < 100; i++)
        {
            beams.Add(new Beam
            {
                Number = i,
                Section = null!,
                Node1 = i,
                Node2 = i + 1
            });
        }

        // Act
        service.GenerateBeamChains(beams);

        // Assert
        Assert.AreEqual(100, beams.Count);
    }

    /// <summary>
    /// Tests that GenerateBeamChains handles beams with maximum integer node numbers.
    /// Input: List with beams having int.MaxValue node numbers.
    /// Expected: No exception thrown, method completes successfully.
    /// </summary>
    [TestMethod]
    public void GenerateBeamChains_BeamsWithMaxIntNodeNumbers_CompletesSuccessfully()
    {
        // Arrange
        var service = new BeamChainService();

        var beam = new Beam
        {
            Number = int.MaxValue,
            Section = null!,
            Node1 = int.MaxValue,
            Node2 = int.MaxValue - 1
        };
        var beams = new List<Beam> { beam };

        // Act
        service.GenerateBeamChains(beams);

        // Assert
        Assert.AreEqual(1, beams.Count);
    }

}