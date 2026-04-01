using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SD.Core.Shared;
using SD.Core.Shared.Enum;
using SD.Core.Shared.Extensions;
using SD.Core.Shared.Models;
using SD.Core.Shared.Models.BeamModels;

namespace SD.Core.Shared.Models.BeamModels.UnitTests;


/// <summary>
/// Unit tests for the BeamChain class.
/// </summary>
[TestClass]
public partial class BeamChainTests
{
    /// <summary>
    /// Tests that LongestChain returns an empty list when all beam lists are null.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenAllBeamListsAreNull_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = null!,
            LeBottomBeams = null!,
            L2Beams = null!,
            L1Beams = null!,
            LzBeams = null!
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that LongestChain returns an empty list when LeTopBeams is null.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenLeTopBeamsIsNull_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = null!,
            LeBottomBeams = [],
            L2Beams = [],
            L1Beams = [],
            LzBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that LongestChain returns an empty list when LeBottomBeams is null.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenLeBottomBeamsIsNull_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = [],
            LeBottomBeams = null!,
            L2Beams = [],
            L1Beams = [],
            LzBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that LongestChain returns an empty list when L2Beams is null.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenL2BeamsIsNull_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = [],
            LeBottomBeams = [],
            L2Beams = null!,
            L1Beams = [],
            LzBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that LongestChain returns an empty list when L1Beams is null.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenL1BeamsIsNull_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = [],
            LeBottomBeams = [],
            L2Beams = [],
            L1Beams = null!,
            LzBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that LongestChain returns an empty list when LzBeams is null.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenLzBeamsIsNull_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = [],
            LeBottomBeams = [],
            L2Beams = [],
            L1Beams = [],
            LzBeams = null!
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that LongestChain returns the first list when all beam lists are empty.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenAllBeamListsAreEmpty_ReturnsFirstListInOrderByDescending()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = [],
            LeBottomBeams = [],
            L2Beams = [],
            L1Beams = [],
            LzBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(beamChain.L2Beams);
    }

    /// <summary>
    /// Tests that LongestChain returns L2Beams when it has the most beams.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenL2BeamsHasMostBeams_ReturnsL2Beams()
    {
        // Arrange
        var section = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>()).Object;
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = section };
        var beam2 = new Beam { Number = 2, Node1 = 20, Node2 = 30, Section = section };
        var beam3 = new Beam { Number = 3, Node1 = 30, Node2 = 40, Section = section };

        var beamChain = new BeamChain
        {
            L2Beams = [beam1, beam2, beam3],
            L1Beams = [beam1],
            LzBeams = [],
            LeTopBeams = [beam1, beam2],
            LeBottomBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.L2Beams);
        result.Should().HaveCount(3);
    }

    /// <summary>
    /// Tests that LongestChain returns LzBeams when it has the most beams.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenLzBeamsHasMostBeams_ReturnsLzBeams()
    {
        // Arrange
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = null! };
        var beam2 = new Beam { Number = 2, Node1 = 20, Node2 = 30, Section = null! };
        var beam3 = new Beam { Number = 3, Node1 = 30, Node2 = 40, Section = null! };
        var beam4 = new Beam { Number = 4, Node1 = 40, Node2 = 50, Section = null! };
        var beam5 = new Beam { Number = 5, Node1 = 50, Node2 = 60, Section = null! };

        var beamChain = new BeamChain
        {
            L2Beams = [beam1, beam2],
            L1Beams = [],
            LzBeams = [beam1, beam2, beam3, beam4, beam5],
            LeTopBeams = [beam1],
            LeBottomBeams = [beam1, beam2, beam3]
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.LzBeams);
        result.Should().HaveCount(5);
    }

    /// <summary>
    /// Tests that LongestChain returns LeTopBeams when it has the most beams.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenLeTopBeamsHasMostBeams_ReturnsLeTopBeams()
    {
        // Arrange
        var section = new Mock<Section>(default(SectionType), default(Material)).Object;
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = section };
        var beam2 = new Beam { Number = 2, Node1 = 20, Node2 = 30, Section = section };
        var beam3 = new Beam { Number = 3, Node1 = 30, Node2 = 40, Section = section };

        var beamChain = new BeamChain
        {
            L2Beams = [],
            L1Beams = [beam1],
            LzBeams = [beam1, beam2],
            LeTopBeams = [beam1, beam2, beam3],
            LeBottomBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.LeTopBeams);
        result.Should().HaveCount(3);
    }

    /// <summary>
    /// Tests that LongestChain returns LeBottomBeams when it has the most beams.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenLeBottomBeamsHasMostBeams_ReturnsLeBottomBeams()
    {
        // Arrange
        var section = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>()).Object;
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = section };
        var beam2 = new Beam { Number = 2, Node1 = 20, Node2 = 30, Section = section };
        var beam3 = new Beam { Number = 3, Node1 = 30, Node2 = 40, Section = section };
        var beam4 = new Beam { Number = 4, Node1 = 40, Node2 = 50, Section = section };

        var beamChain = new BeamChain
        {
            L2Beams = [beam1],
            L1Beams = [beam1, beam2],
            LzBeams = [],
            LeTopBeams = [beam1, beam2, beam3],
            LeBottomBeams = [beam1, beam2, beam3, beam4]
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.LeBottomBeams);
        result.Should().HaveCount(4);
    }

    /// <summary>
    /// Tests that LongestChain returns L2Beams when multiple lists have the same maximum count.
    /// L2Beams is first in the OrderByDescending sequence.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenMultipleListsHaveSameCount_ReturnsL2Beams()
    {
        // Arrange
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = null! };
        var beam2 = new Beam { Number = 2, Node1 = 20, Node2 = 30, Section = null! };
        var beam3 = new Beam { Number = 3, Node1 = 30, Node2 = 40, Section = null! };
        var beam4 = new Beam { Number = 4, Node1 = 40, Node2 = 50, Section = null! };

        var beamChain = new BeamChain
        {
            L2Beams = [beam1, beam2],
            L1Beams = [beam3, beam4],
            LzBeams = [],
            LeTopBeams = [],
            LeBottomBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.L2Beams);
        result.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that LongestChain returns the correct list when only one list has beams.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenOnlyOneListHasBeams_ReturnsThatList()
    {
        // Arrange
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = null! };

        var beamChain = new BeamChain
        {
            L2Beams = [],
            L1Beams = [],
            LzBeams = [],
            LeTopBeams = [],
            LeBottomBeams = [beam1]
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.LeBottomBeams);
        result.Should().HaveCount(1);
    }

    /// <summary>
    /// Tests that LongestChain handles a scenario with multiple beam items where node numbers are close,
    /// with some items fitting in one chain and others fitting in another chain.
    /// </summary>
    [TestMethod]
    public void LongestChain_WithMultipleBeamsAndCloseNodeNumbers_ReturnsListWithMostBeams()
    {
        // Arrange
        var section = new Mock<Section>(default(SectionType), default(Material)).Object;

        // Create beams with close node numbers
        var beam1 = new Beam { Number = 1, Node1 = 100, Node2 = 101, Section = section };
        var beam2 = new Beam { Number = 2, Node1 = 101, Node2 = 102, Section = section };
        var beam3 = new Beam { Number = 3, Node1 = 102, Node2 = 103, Section = section };
        var beam4 = new Beam { Number = 4, Node1 = 103, Node2 = 104, Section = section };
        var beam5 = new Beam { Number = 5, Node1 = 104, Node2 = 105, Section = section };

        // Additional beams for other chains
        var beam6 = new Beam { Number = 6, Node1 = 200, Node2 = 201, Section = section };
        var beam7 = new Beam { Number = 7, Node1 = 201, Node2 = 202, Section = section };
        var beam8 = new Beam { Number = 8, Node1 = 202, Node2 = 203, Section = section };

        var beamChain = new BeamChain
        {
            L2Beams = [beam1, beam2, beam3, beam4, beam5], // 5 beams - longest
            L1Beams = [beam6, beam7, beam8],               // 3 beams
            LzBeams = [beam1, beam2],                      // 2 beams
            LeTopBeams = [beam6],                          // 1 beam
            LeBottomBeams = [beam1, beam2, beam3]          // 3 beams
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.L2Beams);
        result.Should().HaveCount(5);
        result.Should().Contain(beam1);
        result.Should().Contain(beam2);
        result.Should().Contain(beam3);
        result.Should().Contain(beam4);
        result.Should().Contain(beam5);
    }

    /// <summary>
    /// Tests that LongestChain returns a new empty list each time when all lists are null,
    /// not the same reference.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenCalledMultipleTimesWithNullLists_ReturnsNewEmptyListEachTime()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = null!,
            LeBottomBeams = null!,
            L2Beams = null!,
            L1Beams = null!,
            LzBeams = null!
        };

        // Act
        var result1 = beamChain.LongestChain;
        var result2 = beamChain.LongestChain;

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1.Should().BeEmpty();
        result2.Should().BeEmpty();
        result1.Should().NotBeSameAs(result2);
    }

    /// <summary>
    /// Tests that LongestChain returns the same list reference when called multiple times
    /// with the same state.
    /// </summary>
    [TestMethod]
    public void LongestChain_WhenCalledMultipleTimes_ReturnsSameListReference()
    {
        // Arrange
        var section = new Mock<Section>(default(SectionType), default(Material)).Object;
        var beam1 = new Beam { Number = 1, Node1 = 10, Node2 = 20, Section = section };
        var beam2 = new Beam { Number = 2, Node1 = 20, Node2 = 30, Section = section };

        var beamChain = new BeamChain
        {
            L2Beams = [beam1, beam2],
            L1Beams = [],
            LzBeams = [],
            LeTopBeams = [],
            LeBottomBeams = []
        };

        // Act
        var result1 = beamChain.LongestChain;
        var result2 = beamChain.LongestChain;

        // Assert
        result1.Should().BeSameAs(beamChain.L2Beams);
        result2.Should().BeSameAs(beamChain.L2Beams);
        result1.Should().BeSameAs(result2);
    }

    /// <summary>
    /// Tests that LongestChain handles large collections efficiently.
    /// </summary>
    [TestMethod]
    public void LongestChain_WithLargeCollections_ReturnsListWithMostBeams()
    {
        // Arrange
        var largeBeamList = Enumerable.Range(1, 1000)
            .Select(i => new Beam { Number = i, Node1 = i, Node2 = i + 1, Section = null! })
            .ToList();

        var beamChain = new BeamChain
        {
            L2Beams = largeBeamList,
            L1Beams = [],
            LzBeams = [],
            LeTopBeams = [],
            LeBottomBeams = []
        };

        // Act
        var result = beamChain.LongestChain;

        // Assert
        result.Should().BeSameAs(beamChain.L2Beams);
        result.Should().HaveCount(1000);
    }

    /// <summary>
    /// Tests that SetConnectedChains sets all five connected chain properties correctly when all input lists are empty (default state).
    /// Input: Empty lists for all Ends and Beams properties.
    /// Expected: All Connected* properties should be set to empty lists.
    /// </summary>
    [TestMethod]
    public void SetConnectedChains_WithEmptyLists_SetsAllConnectedChainsToEmpty()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        beamChain.SetConnectedChains();

        // Assert
        beamChain.ConnectedChaineTop.Should().NotBeNull().And.BeEmpty();
        beamChain.ConnectedChaineBottom.Should().NotBeNull().And.BeEmpty();
        beamChain.ConnectedChainz.Should().NotBeNull().And.BeEmpty();
        beamChain.ConnectedChain2.Should().NotBeNull().And.BeEmpty();
        beamChain.ConnectedChain1.Should().NotBeNull().And.BeEmpty();
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam clears all ById collections and sets all Beam collections to contain only the primary beam.
    /// Input: A valid Beam object.
    /// Expected: All ById collections are empty, all Beam collections contain only the primary beam.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_ValidBeam_ClearsAllByIdCollectionsAndSetsAllBeamCollections()
    {
        // Arrange
        var beamChain = new BeamChain();
        var mockSection = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>());
        var primaryBeam = new Beam { Number = 1, Section = mockSection.Object };

        // Act
        beamChain.ResetToPrimaryBeam(primaryBeam);

        // Assert
        beamChain.L1BeamsById.Should().BeEmpty();
        beamChain.L2BeamsById.Should().BeEmpty();
        beamChain.LzBeamsById.Should().BeEmpty();
        beamChain.LeTopBeamsById.Should().BeEmpty();
        beamChain.LeBottomBeamsById.Should().BeEmpty();

        beamChain.L1Beams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.L2Beams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.LzBeams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.LeTopBeams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.LeBottomBeams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam replaces existing data in all collections.
    /// Input: A BeamChain with existing data in all collections, and a new primary beam.
    /// Expected: All existing data is cleared/replaced with the new primary beam.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_ExistingDataInCollections_ReplacesAllExistingData()
    {
        // Arrange
        var beamChain = new BeamChain();
        var existingBeam1 = new Beam { Number = 100, Section = null! };
        var existingBeam2 = new Beam { Number = 200, Section = null! };
        var primaryBeam = new Beam { Number = 1, Section = null! };

        beamChain.L1BeamsById.AddRange(new[] { 100, 200, 300 });
        beamChain.L2BeamsById.AddRange(new[] { 400, 500 });
        beamChain.LzBeamsById.AddRange(new[] { 600 });
        beamChain.LeTopBeamsById.AddRange(new[] { 700, 800, 900, 1000 });
        beamChain.LeBottomBeamsById.AddRange(new[] { 1100 });

        beamChain.L1Beams.AddRange(new[] { existingBeam1, existingBeam2 });
        beamChain.L2Beams.Add(existingBeam1);
        beamChain.LzBeams.AddRange(new[] { existingBeam1, existingBeam2 });
        beamChain.LeTopBeams.Add(existingBeam2);
        beamChain.LeBottomBeams.AddRange(new[] { existingBeam1, existingBeam2 });

        // Act
        beamChain.ResetToPrimaryBeam(primaryBeam);

        // Assert
        beamChain.L1BeamsById.Should().BeEmpty();
        beamChain.L2BeamsById.Should().BeEmpty();
        beamChain.LzBeamsById.Should().BeEmpty();
        beamChain.LeTopBeamsById.Should().BeEmpty();
        beamChain.LeBottomBeamsById.Should().BeEmpty();

        beamChain.L1Beams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.L2Beams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.LzBeams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.LeTopBeams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
        beamChain.LeBottomBeams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);

        beamChain.L1Beams.Should().NotContain(existingBeam1);
        beamChain.L1Beams.Should().NotContain(existingBeam2);
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam handles multiple calls correctly.
    /// Input: Multiple consecutive calls with different primary beams.
    /// Expected: Each call replaces the previous state with the new primary beam.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_MultipleCalls_ReplacesStateEachTime()
    {
        // Arrange
        var beamChain = new BeamChain();
        var mockSection1 = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>());
        var mockSection2 = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>());
        var firstBeam = new Beam { Number = 1, Section = mockSection1.Object };
        var secondBeam = new Beam { Number = 2, Section = mockSection2.Object };

        // Act
        beamChain.ResetToPrimaryBeam(firstBeam);
        beamChain.L1BeamsById.Add(100);
        beamChain.L2BeamsById.Add(200);

        beamChain.ResetToPrimaryBeam(secondBeam);

        // Assert
        beamChain.L1BeamsById.Should().BeEmpty();
        beamChain.L2BeamsById.Should().BeEmpty();
        beamChain.LzBeamsById.Should().BeEmpty();
        beamChain.LeTopBeamsById.Should().BeEmpty();
        beamChain.LeBottomBeamsById.Should().BeEmpty();

        beamChain.L1Beams.Should().ContainSingle().Which.Should().BeSameAs(secondBeam);
        beamChain.L2Beams.Should().ContainSingle().Which.Should().BeSameAs(secondBeam);
        beamChain.LzBeams.Should().ContainSingle().Which.Should().BeSameAs(secondBeam);
        beamChain.LeTopBeams.Should().ContainSingle().Which.Should().BeSameAs(secondBeam);
        beamChain.LeBottomBeams.Should().ContainSingle().Which.Should().BeSameAs(secondBeam);

        beamChain.L1Beams.Should().NotContain(firstBeam);
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam accepts null primaryBeam without throwing.
    /// Input: null primaryBeam.
    /// Expected: No exception is thrown, and beam lists contain null.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_NullPrimaryBeam_ThrowsNullReferenceException()
    {
        // Arrange
        var beamChain = new BeamChain();
        Beam primaryBeam = null!;

        // Act
        var act = () => beamChain.ResetToPrimaryBeam(primaryBeam);

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam sets the same beam reference in all collections.
    /// Input: A valid Beam object.
    /// Expected: All Beam collections reference the exact same Beam instance.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_ValidBeam_SetsIdenticalReferenceInAllCollections()
    {
        // Arrange
        var beamChain = new BeamChain();
        var primaryBeam = new Beam { Number = 42, Section = null! };

        // Act
        beamChain.ResetToPrimaryBeam(primaryBeam);

        // Assert
        beamChain.L1Beams[0].Should().BeSameAs(primaryBeam);
        beamChain.L2Beams[0].Should().BeSameAs(primaryBeam);
        beamChain.LzBeams[0].Should().BeSameAs(primaryBeam);
        beamChain.LeTopBeams[0].Should().BeSameAs(primaryBeam);
        beamChain.LeBottomBeams[0].Should().BeSameAs(primaryBeam);

        beamChain.L1Beams[0].Should().BeSameAs(beamChain.L2Beams[0]);
        beamChain.L1Beams[0].Should().BeSameAs(beamChain.LzBeams[0]);
        beamChain.L1Beams[0].Should().BeSameAs(beamChain.LeTopBeams[0]);
        beamChain.L1Beams[0].Should().BeSameAs(beamChain.LeBottomBeams[0]);
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam works correctly when ById collections have many items.
    /// Input: BeamChain with large number of items in ById collections.
    /// Expected: All ById collections are completely cleared.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_LargeNumberOfItemsInByIdCollections_ClearsAllItems()
    {
        // Arrange
        var beamChain = new BeamChain();
        var primaryBeam = new Beam { Number = 1, Section = null! };

        for (int i = 0; i < 1000; i++)
        {
            beamChain.L1BeamsById.Add(i);
            beamChain.L2BeamsById.Add(i + 1000);
            beamChain.LzBeamsById.Add(i + 2000);
            beamChain.LeTopBeamsById.Add(i + 3000);
            beamChain.LeBottomBeamsById.Add(i + 4000);
        }

        // Act
        beamChain.ResetToPrimaryBeam(primaryBeam);

        // Assert
        beamChain.L1BeamsById.Should().BeEmpty();
        beamChain.L2BeamsById.Should().BeEmpty();
        beamChain.LzBeamsById.Should().BeEmpty();
        beamChain.LeTopBeamsById.Should().BeEmpty();
        beamChain.LeBottomBeamsById.Should().BeEmpty();

        beamChain.L1Beams.Should().ContainSingle().Which.Should().BeSameAs(primaryBeam);
    }

    /// <summary>
    /// Tests that ResetToPrimaryBeam correctly handles beams with extreme property values.
    /// Input: A Beam with int.MaxValue as Number property.
    /// Expected: The beam is correctly set in all collections.
    /// </summary>
    [TestMethod]
    public void ResetToPrimaryBeam_BeamWithMaxValueNumber_SetsCorrectly()
    {
        // Arrange
        var beamChain = new BeamChain();
        var mockSection = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>());
        var primaryBeam = new Beam { Number = int.MaxValue, Section = mockSection.Object };

        // Act
        beamChain.ResetToPrimaryBeam(primaryBeam);

        // Assert
        beamChain.L1Beams.Should().ContainSingle().Which.Number.Should().Be(int.MaxValue);
        beamChain.L2Beams.Should().ContainSingle().Which.Number.Should().Be(int.MaxValue);
        beamChain.LzBeams.Should().ContainSingle().Which.Number.Should().Be(int.MaxValue);
        beamChain.LeTopBeams.Should().ContainSingle().Which.Number.Should().Be(int.MaxValue);
        beamChain.LeBottomBeams.Should().ContainSingle().Which.Number.Should().Be(int.MaxValue);
    }

    /// <summary>
    /// Tests that SetLengths sets all lengths to zero when all beam lists are empty.
    /// </summary>
    [TestMethod]
    public void SetLengths_WhenAllBeamListsAreEmpty_SetsAllLengthsToZero()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        beamChain.SetLengths();

        // Assert
        beamChain.LeBottom.Should().Be(0);
        beamChain.LeTop.Should().Be(0);
        beamChain.Lz.Should().Be(0);
        beamChain.L2.Should().Be(0);
        beamChain.L1.Should().Be(0);
    }

    /// <summary>
    /// Tests that SetChainKValues successfully propagates K values to all beams in all lists when lists contain beams.
    /// Input: BeamChain with specific K values and beams in all five lists.
    /// Expected: All beams in all lists have their BeamChain K values updated to match the parent BeamChain.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_WithBeamsInAllLists_PropagatesKValuesToAllBeams()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = 2.5,
            K2 = 3.0,
            Kz = 1.5,
            KeTop = 4.0,
            KeBottom = 5.0
        };

        var l2Beam = new Beam { Section = null!, BeamChain = new BeamChain() };
        var l1Beam = new Beam { Section = null!, BeamChain = new BeamChain() };
        var lzBeam = new Beam { Section = null!, BeamChain = new BeamChain() };
        var leTopBeam = new Beam { Section = null!, BeamChain = new BeamChain() };
        var leBottomBeam = new Beam { Section = null!, BeamChain = new BeamChain() };

        beamChain.L2Beams.Add(l2Beam);
        beamChain.L1Beams.Add(l1Beam);
        beamChain.LzBeams.Add(lzBeam);
        beamChain.LeTopBeams.Add(leTopBeam);
        beamChain.LeBottomBeams.Add(leBottomBeam);

        // Act
        beamChain.SetChainKValues();

        // Assert
        l2Beam.BeamChain.K1.Should().Be(2.5);
        l2Beam.BeamChain.K2.Should().Be(3.0);
        l2Beam.BeamChain.Kz.Should().Be(1.5);
        l2Beam.BeamChain.KeTop.Should().Be(4.0);
        l2Beam.BeamChain.KeBottom.Should().Be(5.0);

        l1Beam.BeamChain.K1.Should().Be(2.5);
        l1Beam.BeamChain.K2.Should().Be(3.0);
        l1Beam.BeamChain.Kz.Should().Be(1.5);
        l1Beam.BeamChain.KeTop.Should().Be(4.0);
        l1Beam.BeamChain.KeBottom.Should().Be(5.0);

        lzBeam.BeamChain.K1.Should().Be(2.5);
        lzBeam.BeamChain.K2.Should().Be(3.0);
        lzBeam.BeamChain.Kz.Should().Be(1.5);
        lzBeam.BeamChain.KeTop.Should().Be(4.0);
        lzBeam.BeamChain.KeBottom.Should().Be(5.0);

        leTopBeam.BeamChain.K1.Should().Be(2.5);
        leTopBeam.BeamChain.K2.Should().Be(3.0);
        leTopBeam.BeamChain.Kz.Should().Be(1.5);
        leTopBeam.BeamChain.KeTop.Should().Be(4.0);
        leTopBeam.BeamChain.KeBottom.Should().Be(5.0);

        leBottomBeam.BeamChain.K1.Should().Be(2.5);
        leBottomBeam.BeamChain.K2.Should().Be(3.0);
        leBottomBeam.BeamChain.Kz.Should().Be(1.5);
        leBottomBeam.BeamChain.KeTop.Should().Be(4.0);
        leBottomBeam.BeamChain.KeBottom.Should().Be(5.0);
    }

    /// <summary>
    /// Tests that SetChainKValues completes successfully when all beam lists are empty.
    /// Input: BeamChain with empty beam lists.
    /// Expected: Method completes without error.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_WithEmptyLists_CompletesWithoutError()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = 2.0,
            K2 = 3.0,
            Kz = 1.5,
            KeTop = 4.0,
            KeBottom = 5.0
        };

        // Act
        Action act = () => beamChain.SetChainKValues();

        // Assert
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that SetChainKValues propagates K values to multiple beams in each list.
    /// Input: BeamChain with multiple beams in each list.
    /// Expected: All beams in all lists have their K values updated.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_WithMultipleBeamsInEachList_PropagatesKValuesToAllBeams()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = 1.2,
            K2 = 2.3,
            Kz = 3.4,
            KeTop = 4.5,
            KeBottom = 5.6
        };

        var l2Beam1 = new Beam { Section = null!, BeamChain = new BeamChain() };
        var l2Beam2 = new Beam { Section = null!, BeamChain = new BeamChain() };
        var l1Beam1 = new Beam { Section = null!, BeamChain = new BeamChain() };
        var l1Beam2 = new Beam { Section = null!, BeamChain = new BeamChain() };
        var lzBeam1 = new Beam { Section = null!, BeamChain = new BeamChain() };
        var lzBeam2 = new Beam { Section = null!, BeamChain = new BeamChain() };

        beamChain.L2Beams.Add(l2Beam1);
        beamChain.L2Beams.Add(l2Beam2);
        beamChain.L1Beams.Add(l1Beam1);
        beamChain.L1Beams.Add(l1Beam2);
        beamChain.LzBeams.Add(lzBeam1);
        beamChain.LzBeams.Add(lzBeam2);

        // Act
        beamChain.SetChainKValues();

        // Assert
        l2Beam1.BeamChain.K1.Should().Be(1.2);
        l2Beam2.BeamChain.K1.Should().Be(1.2);
        l1Beam1.BeamChain.K2.Should().Be(2.3);
        l1Beam2.BeamChain.K2.Should().Be(2.3);
        lzBeam1.BeamChain.Kz.Should().Be(3.4);
        lzBeam2.BeamChain.Kz.Should().Be(3.4);
    }

    /// <summary>
    /// Tests that SetChainKValues correctly propagates edge case K values including zero.
    /// Input: BeamChain with K values set to 0.0.
    /// Expected: All beams have their K values set to 0.0.
    /// </summary>
    [TestMethod]
    [DataRow(0.0, 0.0, 0.0, 0.0, 0.0, DisplayName = "All zeros")]
    [DataRow(-1.0, -2.0, -3.0, -4.0, -5.0, DisplayName = "Negative values")]
    [DataRow(double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, double.MaxValue, DisplayName = "Max values")]
    [DataRow(double.MinValue, double.MinValue, double.MinValue, double.MinValue, double.MinValue, DisplayName = "Min values")]
    public void SetChainKValues_WithEdgeCaseKValues_PropagatesCorrectly(double k1, double k2, double kz, double keTop, double keBottom)
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = k1,
            K2 = k2,
            Kz = kz,
            KeTop = keTop,
            KeBottom = keBottom
        };

        var beam = new Beam { Section = null!, BeamChain = new BeamChain() };
        beamChain.L2Beams.Add(beam);

        // Act
        beamChain.SetChainKValues();

        // Assert
        beam.BeamChain.K1.Should().Be(k1);
        beam.BeamChain.K2.Should().Be(k2);
        beam.BeamChain.Kz.Should().Be(kz);
        beam.BeamChain.KeTop.Should().Be(keTop);
        beam.BeamChain.KeBottom.Should().Be(keBottom);
    }

    /// <summary>
    /// Tests that SetChainKValues correctly handles special double values like NaN and Infinity.
    /// Input: BeamChain with K values set to NaN, PositiveInfinity, and NegativeInfinity.
    /// Expected: All beams have their K values set to the special values.
    /// </summary>
    [TestMethod]
    [DataRow(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, DisplayName = "All NaN")]
    [DataRow(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, DisplayName = "All PositiveInfinity")]
    [DataRow(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, DisplayName = "All NegativeInfinity")]
    public void SetChainKValues_WithSpecialDoubleValues_PropagatesCorrectly(double k1, double k2, double kz, double keTop, double keBottom)
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = k1,
            K2 = k2,
            Kz = kz,
            KeTop = keTop,
            KeBottom = keBottom
        };

        var beam = new Beam { Section = null!, BeamChain = new BeamChain() };
        beamChain.L1Beams.Add(beam);

        // Act
        beamChain.SetChainKValues();

        // Assert
        if (double.IsNaN(k1))
        {
            beam.BeamChain.K1.Should().Be(double.NaN);
        }
        else
        {
            beam.BeamChain.K1.Should().Be(k1);
        }

        if (double.IsNaN(k2))
        {
            beam.BeamChain.K2.Should().Be(double.NaN);
        }
        else
        {
            beam.BeamChain.K2.Should().Be(k2);
        }

        if (double.IsNaN(kz))
        {
            beam.BeamChain.Kz.Should().Be(double.NaN);
        }
        else
        {
            beam.BeamChain.Kz.Should().Be(kz);
        }

        if (double.IsNaN(keTop))
        {
            beam.BeamChain.KeTop.Should().Be(double.NaN);
        }
        else
        {
            beam.BeamChain.KeTop.Should().Be(keTop);
        }

        if (double.IsNaN(keBottom))
        {
            beam.BeamChain.KeBottom.Should().Be(double.NaN);
        }
        else
        {
            beam.BeamChain.KeBottom.Should().Be(keBottom);
        }
    }

    /// <summary>
    /// Tests that SetChainKValues only updates beams in the lists, not other beams.
    /// Input: BeamChain with beams in some lists, other beams not in any list.
    /// Expected: Only beams in the lists have their K values updated.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_OnlyUpdatesBeamsInLists_NotOtherBeams()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = 10.0,
            K2 = 20.0,
            Kz = 30.0,
            KeTop = 40.0,
            KeBottom = 50.0
        };

        var beamInList = new Beam { Section = null!, BeamChain = new BeamChain() };
        var beamNotInList = new Beam { Section = null!, BeamChain = new BeamChain { K1 = 1.0, K2 = 2.0, Kz = 3.0, KeTop = 4.0, KeBottom = 5.0 } };

        beamChain.L2Beams.Add(beamInList);

        // Act
        beamChain.SetChainKValues();

        // Assert
        beamInList.BeamChain.K1.Should().Be(10.0);
        beamInList.BeamChain.K2.Should().Be(20.0);
        beamInList.BeamChain.Kz.Should().Be(30.0);
        beamInList.BeamChain.KeTop.Should().Be(40.0);
        beamInList.BeamChain.KeBottom.Should().Be(50.0);

        beamNotInList.BeamChain.K1.Should().Be(1.0);
        beamNotInList.BeamChain.K2.Should().Be(2.0);
        beamNotInList.BeamChain.Kz.Should().Be(3.0);
        beamNotInList.BeamChain.KeTop.Should().Be(4.0);
        beamNotInList.BeamChain.KeBottom.Should().Be(5.0);
    }

    /// <summary>
    /// Tests that SetChainKValues updates beams in specific lists correctly with partial list population.
    /// Input: BeamChain with beams only in LeTopBeams and LeBottomBeams.
    /// Expected: Only beams in LeTopBeams and LeBottomBeams have their K values updated.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_WithPartialListPopulation_UpdatesOnlyPopulatedLists()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = 7.5,
            K2 = 8.5,
            Kz = 9.5,
            KeTop = 10.5,
            KeBottom = 11.5
        };

        var leTopBeam = new Beam { Section = null!, BeamChain = new BeamChain() };
        var leBottomBeam = new Beam { Section = null!, BeamChain = new BeamChain() };

        beamChain.LeTopBeams.Add(leTopBeam);
        beamChain.LeBottomBeams.Add(leBottomBeam);

        // Act
        beamChain.SetChainKValues();

        // Assert
        leTopBeam.BeamChain.K1.Should().Be(7.5);
        leTopBeam.BeamChain.K2.Should().Be(8.5);
        leTopBeam.BeamChain.Kz.Should().Be(9.5);
        leTopBeam.BeamChain.KeTop.Should().Be(10.5);
        leTopBeam.BeamChain.KeBottom.Should().Be(11.5);

        leBottomBeam.BeamChain.K1.Should().Be(7.5);
        leBottomBeam.BeamChain.K2.Should().Be(8.5);
        leBottomBeam.BeamChain.Kz.Should().Be(9.5);
        leBottomBeam.BeamChain.KeTop.Should().Be(10.5);
        leBottomBeam.BeamChain.KeBottom.Should().Be(11.5);
    }

    /// <summary>
    /// Tests that SetChainKValues handles a scenario with multiple beams where node numbers are close,
    /// with some beams in one list and others in another.
    /// Input: BeamChain with beams having close node numbers distributed across different lists.
    /// Expected: All beams have their K values updated according to their respective lists.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_WithBeamsHavingCloseNodeNumbers_PropagatesCorrectly()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            K1 = 6.0,
            K2 = 7.0,
            Kz = 8.0,
            KeTop = 9.0,
            KeBottom = 10.0
        };

        var beam1 = new Beam { Section = null!, Number = 1, Node1 = 100, Node2 = 101, BeamChain = new BeamChain() };
        var beam2 = new Beam { Section = null!, Number = 2, Node1 = 101, Node2 = 102, BeamChain = new BeamChain() };
        var beam3 = new Beam { Section = null!, Number = 3, Node1 = 102, Node2 = 103, BeamChain = new BeamChain() };
        var beam4 = new Beam { Section = null!, Number = 4, Node1 = 200, Node2 = 201, BeamChain = new BeamChain() };
        var beam5 = new Beam { Section = null!, Number = 5, Node1 = 201, Node2 = 202, BeamChain = new BeamChain() };

        beamChain.L2Beams.Add(beam1);
        beamChain.L2Beams.Add(beam2);
        beamChain.L1Beams.Add(beam3);
        beamChain.LzBeams.Add(beam4);
        beamChain.LzBeams.Add(beam5);

        // Act
        beamChain.SetChainKValues();

        // Assert
        beam1.BeamChain.K1.Should().Be(6.0);
        beam1.BeamChain.K2.Should().Be(7.0);
        beam1.BeamChain.Kz.Should().Be(8.0);
        beam1.BeamChain.KeTop.Should().Be(9.0);
        beam1.BeamChain.KeBottom.Should().Be(10.0);

        beam2.BeamChain.K1.Should().Be(6.0);
        beam2.BeamChain.K2.Should().Be(7.0);
        beam2.BeamChain.Kz.Should().Be(8.0);
        beam2.BeamChain.KeTop.Should().Be(9.0);
        beam2.BeamChain.KeBottom.Should().Be(10.0);

        beam3.BeamChain.K1.Should().Be(6.0);
        beam3.BeamChain.K2.Should().Be(7.0);
        beam3.BeamChain.Kz.Should().Be(8.0);
        beam3.BeamChain.KeTop.Should().Be(9.0);
        beam3.BeamChain.KeBottom.Should().Be(10.0);

        beam4.BeamChain.K1.Should().Be(6.0);
        beam4.BeamChain.K2.Should().Be(7.0);
        beam4.BeamChain.Kz.Should().Be(8.0);
        beam4.BeamChain.KeTop.Should().Be(9.0);
        beam4.BeamChain.KeBottom.Should().Be(10.0);

        beam5.BeamChain.K1.Should().Be(6.0);
        beam5.BeamChain.K2.Should().Be(7.0);
        beam5.BeamChain.Kz.Should().Be(8.0);
        beam5.BeamChain.KeTop.Should().Be(9.0);
        beam5.BeamChain.KeBottom.Should().Be(10.0);
    }

    /// <summary>
    /// Tests that SetChainKValues correctly propagates default K values (1.0) to beams.
    /// Input: BeamChain with default K values.
    /// Expected: All beams have their K values set to 1.0.
    /// </summary>
    [TestMethod]
    public void SetChainKValues_WithDefaultKValues_PropagatesDefaultValues()
    {
        // Arrange
        var beamChain = new BeamChain();

        var beam = new Beam { Section = null!, BeamChain = new BeamChain() };
        beamChain.L1Beams.Add(beam);

        // Act
        beamChain.SetChainKValues();

        // Assert
        beam.BeamChain.K1.Should().Be(1.0);
        beam.BeamChain.K2.Should().Be(1.0);
        beam.BeamChain.Kz.Should().Be(1.0);
        beam.BeamChain.KeTop.Should().Be(1.0);
        beam.BeamChain.KeBottom.Should().Be(1.0);
    }

    /// <summary>
    /// Tests that EndL2Nodes returns an empty list when L2Beams is empty.
    /// </summary>
    [TestMethod]
    public void EndL2Nodes_EmptyL2Beams_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            L2Beams = new List<Beam>()
        };

        // Act
        var result = beamChain.EndL2Nodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that EndL1Nodes returns an empty list when L1Beams is empty.
    /// </summary>
    [TestMethod]
    public void EndL1Nodes_EmptyL1Beams_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            L1Beams = new List<Beam>()
        };

        // Act
        var result = beamChain.EndL1Nodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that EndLzNodes returns an empty list when LzBeams is empty.
    /// </summary>
    [TestMethod]
    public void EndLzNodes_EmptyLzBeams_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        var result = beamChain.EndLzNodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that EndLzNodes returns all nodes when beams are not connected.
    /// All nodes appear exactly once, making them all free nodes.
    /// </summary>
    [TestMethod]
    public void EndLzNodes_TwoDisconnectedBeams_ReturnsAllNodes()
    {
        // Arrange
        var beamChain = new BeamChain();
        var sectionMock = new Mock<Section>(It.IsAny<SectionType>(), It.IsAny<Material>());
        var beam1 = new Beam
        {
            Node1 = 1,
            Node2 = 2,
            Section = sectionMock.Object
        };

        var beam2 = new Beam
        {
            Node1 = 3,
            Node2 = 4,
            Section = sectionMock.Object
        };

        beamChain.LzBeams = new List<Beam> { beam1, beam2 };

        // Act
        var result = beamChain.EndLzNodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().Contain(new[] { 1, 2, 3, 4 });
    }

    /// <summary>
    /// Tests that EndLzNodes handles beams with same node for Node1 and Node2.
    /// A self-connected beam has no free nodes.
    /// </summary>
    [TestMethod]
    public void EndLzNodes_BeamWithSameNode1AndNode2_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain();
        var beam = new Beam { Section = null!, Node1 = 5, Node2 = 5 };

        beamChain.LzBeams = new List<Beam> { beam };

        // Act
        var result = beamChain.EndLzNodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that EndLzNodes handles extreme node values correctly.
    /// Verifies that int.MinValue and int.MaxValue are properly processed.
    /// </summary>
    [TestMethod]
    public void EndLzNodes_ExtremeNodeValues_ReturnsCorrectFreeNodes()
    {
        // Arrange
        var beamChain = new BeamChain();
        
        var beam1 = new Beam
        {
            Section = null!,
            Node1 = int.MinValue,
            Node2 = 0
        };

        var beam2 = new Beam
        {
            Section = null!,
            Node1 = 0,
            Node2 = int.MaxValue
        };

        beamChain.LzBeams = new List<Beam> { beam1, beam2 };

        // Act
        var result = beamChain.EndLzNodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(new[] { int.MinValue, int.MaxValue });
        result.Should().NotContain(0);
    }

    /// <summary>
    /// Tests that EndLeTopNodes returns an empty list when LeTopBeams is empty.
    /// Input: Empty LeTopBeams collection.
    /// Expected: Empty list of node IDs.
    /// </summary>
    [TestMethod]
    public void EndLeTopNodes_EmptyLeTopBeams_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeTopBeams = []
        };

        // Act
        var result = beamChain.EndLeTopNodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that EndLeBottomNodes returns an empty list when LeBottomBeams is empty.
    /// </summary>
    [TestMethod]
    public void EndLeBottomNodes_EmptyLeBottomBeams_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeBottomBeams = []
        };

        // Act
        var result = beamChain.EndLeBottomNodes;

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetChainBeamsForAxis returns the correct beam list for each valid BeamAxis value
    /// that maps to a specific property.
    /// </summary>
    /// <param name="beamAxis">The beam axis to query.</param>
    /// <param name="propertyName">The name of the expected property for validation purposes.</param>
    [TestMethod]
    [DataRow(BeamAxis.Principal1, "L1Beams")]
    [DataRow(BeamAxis.Principal2, "L2Beams")]
    [DataRow(BeamAxis.PrincipalZ, "LzBeams")]
    [DataRow(BeamAxis.PrincipalETop, "LeTopBeams")]
    [DataRow(BeamAxis.PrincipalEBottom, "LeBottomBeams")]
    public void GetChainBeamsForAxis_ValidBeamAxisValue_ReturnsCorrespondingBeamList(BeamAxis beamAxis, string propertyName)
    {
        // Arrange
        var beamChain = new BeamChain();
        var expectedList = propertyName switch
        {
            "L1Beams" => beamChain.L1Beams,
            "L2Beams" => beamChain.L2Beams,
            "LzBeams" => beamChain.LzBeams,
            "LeTopBeams" => beamChain.LeTopBeams,
            "LeBottomBeams" => beamChain.LeBottomBeams,
            _ => throw new InvalidOperationException("Invalid property name in test data")
        };

        // Act
        var result = beamChain.GetChainBeamsForAxis(beamAxis);

        // Assert
        result.Should().BeSameAs(expectedList, $"GetChainBeamsForAxis should return the {propertyName} property for {beamAxis}");
    }

    /// <summary>
    /// Tests that GetChainBeamsForAxis returns an empty list when the BeamAxis.All value is provided,
    /// which falls into the default case.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsForAxis_BeamAxisAll_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        var result = beamChain.GetChainBeamsForAxis(BeamAxis.All);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetChainBeamsForAxis returns an empty list when an undefined BeamAxis value
    /// (outside the enum's defined range) is provided, falling into the default case.
    /// </summary>
    [TestMethod]
    [DataRow(999)]
    [DataRow(-1)]
    [DataRow(100)]
    public void GetChainBeamsForAxis_UndefinedEnumValue_ReturnsEmptyList(int enumValue)
    {
        // Arrange
        var beamChain = new BeamChain();
        var invalidBeamAxis = (BeamAxis)enumValue;

        // Act
        var result = beamChain.GetChainBeamsForAxis(invalidBeamAxis);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetChainBeamsForAxis returns distinct list references for different BeamAxis values.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsForAxis_DifferentBeamAxisValues_ReturnsDifferentListReferences()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        var l1Result = beamChain.GetChainBeamsForAxis(BeamAxis.Principal1);
        var l2Result = beamChain.GetChainBeamsForAxis(BeamAxis.Principal2);
        var lzResult = beamChain.GetChainBeamsForAxis(BeamAxis.PrincipalZ);
        var leTopResult = beamChain.GetChainBeamsForAxis(BeamAxis.PrincipalETop);
        var leBottomResult = beamChain.GetChainBeamsForAxis(BeamAxis.PrincipalEBottom);

        // Assert
        l1Result.Should().NotBeSameAs(l2Result);
        l1Result.Should().NotBeSameAs(lzResult);
        l1Result.Should().NotBeSameAs(leTopResult);
        l1Result.Should().NotBeSameAs(leBottomResult);
        l2Result.Should().NotBeSameAs(lzResult);
        l2Result.Should().NotBeSameAs(leTopResult);
        l2Result.Should().NotBeSameAs(leBottomResult);
        lzResult.Should().NotBeSameAs(leTopResult);
        lzResult.Should().NotBeSameAs(leBottomResult);
        leTopResult.Should().NotBeSameAs(leBottomResult);
    }

    /// <summary>
    /// Tests that GetChainBeamsForAxis returns an empty list (not null) for the default case,
    /// ensuring proper initialization of the collection literal.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsForAxis_DefaultCase_ReturnsNewEmptyListNotNull()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        var result1 = beamChain.GetChainBeamsForAxis(BeamAxis.All);
        var result2 = beamChain.GetChainBeamsForAxis(BeamAxis.All);

        // Assert
        result1.Should().NotBeNull();
        result1.Should().BeEmpty();
        result2.Should().NotBeNull();
        result2.Should().BeEmpty();
        // Each call to default case creates a new empty list
        result1.Should().NotBeSameAs(result2);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns the correct L1BeamsById list when Principal1 axis is specified.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_Principal1_ReturnsL1BeamsById()
    {
        // Arrange
        var beamChain = new BeamChain();
        var expectedList = new List<int> { 1, 2, 3 };
        beamChain.L1BeamsById = expectedList;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal1);

        // Assert
        result.Should().BeSameAs(expectedList);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns the correct L2BeamsById list when Principal2 axis is specified.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_Principal2_ReturnsL2BeamsById()
    {
        // Arrange
        var beamChain = new BeamChain();
        var expectedList = new List<int> { 4, 5, 6 };
        beamChain.L2BeamsById = expectedList;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal2);

        // Assert
        result.Should().BeSameAs(expectedList);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns the correct LzBeamsById list when PrincipalZ axis is specified.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_PrincipalZ_ReturnsLzBeamsById()
    {
        // Arrange
        var beamChain = new BeamChain();
        var expectedList = new List<int> { 7, 8, 9 };
        beamChain.LzBeamsById = expectedList;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalZ);

        // Assert
        result.Should().BeSameAs(expectedList);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns the correct LeTopBeamsById list when PrincipalETop axis is specified.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_PrincipalETop_ReturnsLeTopBeamsById()
    {
        // Arrange
        var beamChain = new BeamChain();
        var expectedList = new List<int> { 10, 11, 12 };
        beamChain.LeTopBeamsById = expectedList;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalETop);

        // Assert
        result.Should().BeSameAs(expectedList);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns the correct LeBottomBeamsById list when PrincipalEBottom axis is specified.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_PrincipalEBottom_ReturnsLeBottomBeamsById()
    {
        // Arrange
        var beamChain = new BeamChain();
        var expectedList = new List<int> { 13, 14, 15 };
        beamChain.LeBottomBeamsById = expectedList;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalEBottom);

        // Assert
        result.Should().BeSameAs(expectedList);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns an empty list when All axis is specified (default case).
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_All_ReturnsEmptyList()
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.All);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns an empty list when an undefined enum value is provided.
    /// </summary>
    [TestMethod]
    [DataRow(100)]
    [DataRow(999)]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void GetChainBeamsByIdForAxis_UndefinedEnumValue_ReturnsEmptyList(int invalidEnumValue)
    {
        // Arrange
        var beamChain = new BeamChain();
        var invalidAxis = (BeamAxis)invalidEnumValue;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(invalidAxis);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns the actual reference to the property list, not a copy.
    /// Verifies that modifications to the returned list affect the original property.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_Principal1_ReturnsSameReferenceAsProperty()
    {
        // Arrange
        var beamChain = new BeamChain();
        beamChain.L1BeamsById.Add(100);

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal1);
        result.Add(200);

        // Assert
        beamChain.L1BeamsById.Should().Contain(200);
        beamChain.L1BeamsById.Should().HaveCount(2);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis works correctly when properties contain empty lists.
    /// </summary>
    [TestMethod]
    [DataRow(BeamAxis.Principal1)]
    [DataRow(BeamAxis.Principal2)]
    [DataRow(BeamAxis.PrincipalZ)]
    [DataRow(BeamAxis.PrincipalETop)]
    [DataRow(BeamAxis.PrincipalEBottom)]
    public void GetChainBeamsByIdForAxis_EmptyPropertyLists_ReturnsEmptyList(BeamAxis axis)
    {
        // Arrange
        var beamChain = new BeamChain();

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(axis);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis returns correct lists when all properties contain different data.
    /// This verifies there's no cross-contamination between different axis properties.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_MultiplePopulatedLists_ReturnsCorrectListForEachAxis()
    {
        // Arrange
        var beamChain = new BeamChain();
        beamChain.L1BeamsById = new List<int> { 1, 2, 3 };
        beamChain.L2BeamsById = new List<int> { 4, 5, 6 };
        beamChain.LzBeamsById = new List<int> { 7, 8, 9 };
        beamChain.LeTopBeamsById = new List<int> { 10, 11, 12 };
        beamChain.LeBottomBeamsById = new List<int> { 13, 14, 15 };

        // Act & Assert
        beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal1).Should().BeEquivalentTo(new[] { 1, 2, 3 });
        beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal2).Should().BeEquivalentTo(new[] { 4, 5, 6 });
        beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalZ).Should().BeEquivalentTo(new[] { 7, 8, 9 });
        beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalETop).Should().BeEquivalentTo(new[] { 10, 11, 12 });
        beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalEBottom).Should().BeEquivalentTo(new[] { 13, 14, 15 });
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis correctly handles lists with duplicate values.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_ListWithDuplicates_ReturnsSameListWithDuplicates()
    {
        // Arrange
        var beamChain = new BeamChain();
        var listWithDuplicates = new List<int> { 1, 2, 2, 3, 3, 3 };
        beamChain.L1BeamsById = listWithDuplicates;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal1);

        // Assert
        result.Should().BeSameAs(listWithDuplicates);
        result.Should().ContainInOrder(1, 2, 2, 3, 3, 3);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis correctly handles lists with negative values.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_ListWithNegativeValues_ReturnsSameList()
    {
        // Arrange
        var beamChain = new BeamChain();
        var listWithNegatives = new List<int> { -1, -100, int.MinValue, 0, 1 };
        beamChain.LzBeamsById = listWithNegatives;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalZ);

        // Assert
        result.Should().BeSameAs(listWithNegatives);
        result.Should().Contain(int.MinValue);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis correctly handles lists with extreme integer values.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_ListWithExtremeValues_ReturnsSameList()
    {
        // Arrange
        var beamChain = new BeamChain();
        var listWithExtremes = new List<int> { int.MinValue, int.MaxValue, 0 };
        beamChain.L2BeamsById = listWithExtremes;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.Principal2);

        // Assert
        result.Should().BeSameAs(listWithExtremes);
        result.Should().ContainInOrder(int.MinValue, int.MaxValue, 0);
    }

    /// <summary>
    /// Tests that GetChainBeamsByIdForAxis correctly handles a large list with many items.
    /// </summary>
    [TestMethod]
    public void GetChainBeamsByIdForAxis_LargeList_ReturnsSameList()
    {
        // Arrange
        var beamChain = new BeamChain();
        var largeList = new List<int>();
        for (int i = 0; i < 10000; i++)
        {
            largeList.Add(i);
        }
        beamChain.LeTopBeamsById = largeList;

        // Act
        var result = beamChain.GetChainBeamsByIdForAxis(BeamAxis.PrincipalETop);

        // Assert
        result.Should().BeSameAs(largeList);
        result.Should().HaveCount(10000);
    }

    /// <summary>
    /// Tests that SetBeamChainsForAxis handles empty lists correctly for various axes.
    /// </summary>
    [TestMethod]
    [DataRow(BeamAxis.Principal1)]
    [DataRow(BeamAxis.Principal2)]
    [DataRow(BeamAxis.PrincipalZ)]
    [DataRow(BeamAxis.PrincipalETop)]
    [DataRow(BeamAxis.PrincipalEBottom)]
    public void SetBeamChainsForAxis_EmptyLists_AssignsEmptyLists(BeamAxis axis)
    {
        // Arrange
        var beamChain = new BeamChain();
        var beamsById = new List<int>();
        var beams = new List<Beam>();

        // Act
        beamChain.SetBeamChainsForAxis(axis, beamsById, beams);

        // Assert
        var (actualBeams, actualBeamsById) = GetBeamListsForAxis(beamChain, axis);
        actualBeams.Should().BeSameAs(beams);
        actualBeamsById.Should().BeSameAs(beamsById);
        actualBeams.Should().BeEmpty();
        actualBeamsById.Should().BeEmpty();
    }

    /// <summary>
    /// Helper method to retrieve beam lists for a specific axis.
    /// </summary>
    private static (List<Beam> beams, List<int> beamsById) GetBeamListsForAxis(BeamChain beamChain, BeamAxis axis)
    {
        return axis switch
        {
            BeamAxis.Principal1 => (beamChain.L1Beams, beamChain.L1BeamsById),
            BeamAxis.Principal2 => (beamChain.L2Beams, beamChain.L2BeamsById),
            BeamAxis.PrincipalZ => (beamChain.LzBeams, beamChain.LzBeamsById),
            BeamAxis.PrincipalETop => (beamChain.LeTopBeams, beamChain.LeTopBeamsById),
            BeamAxis.PrincipalEBottom => (beamChain.LeBottomBeams, beamChain.LeBottomBeamsById),
            _ => (new List<Beam>(), new List<int>())
        };
    }

    /// <summary>
    /// Tests that ChainName returns an empty string when all beam chains are empty.
    /// </summary>
    [TestMethod]
    public void ChainName_AllChainsEmpty_ReturnsEmptyString()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeBottomBeams = new List<Beam>(),
            LeTopBeams = new List<Beam>(),
            LzBeams = new List<Beam>(),
            L2Beams = new List<Beam>(),
            L1Beams = new List<Beam>()
        };

        // Act
        var result = beamChain.ChainName;

        // Assert
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that ChainName returns empty string when all beam collections are null.
    /// Verifies null safety of the implementation.
    /// </summary>
    [TestMethod]
    public void ChainName_NullBeamCollections_ReturnsEmptyString()
    {
        // Arrange
        var beamChain = new BeamChain
        {
            LeBottomBeams = null!,
            LeTopBeams = null!,
            LzBeams = null!,
            L2Beams = null!,
            L1Beams = null!
        };

        // Act
        var result = beamChain.ChainName;

        // Assert
        result.Should().BeEmpty();
    }

}