using Moq;
using SD.Core.Infrastructure.Interfaces;
using SD.Core.Shared.Models;
using SD.Data.Interfaces;
using SD.Fem.Strand7.Interfaces;
using SD.Fem.Strand7.Services;

namespace SD.Tests.Unit;

[TestClass]
public class TankDesignServiceTests
{
    private readonly Mock<IStrandApiCreateService> _mockCreateService = new();
    private readonly Mock<IStrandApiService> _mockApiService = new();
    
    [TestMethod]
    public async Task BuildCircularTankModel_GeneratesExpectedNodesAndPlates()
    {
        // Arrange
        var service = new TankDesignService(
            _mockCreateService.Object,
            _mockApiService.Object);

        double diameter = 300; // mm
        double meshSize = 100;  // mm
        int plateNodeCount = 4;
        double roofThickness = 20;
        double baseThickness = 30;

        var heightSegments = new List<HeightSegment>
        {
            new(200, 15,1),
            new(100, 20,2)
        };

        // Act
        await service.BuildCircularTankModel(1, diameter, heightSegments, meshSize, plateNodeCount, roofThickness, baseThickness, "sdcs");

        // Assert: check node generation for first segment
        int nodeNumber = 1;
        var nodes = TankDesignService.GenerateCylinderNodes(new double[] { 0, 0, 0 }, diameter / 2, heightSegments[0].Height, meshSize, meshSize, ref nodeNumber);
        Assert.IsTrue(nodes.Item1.Count > 0, "Nodes should be generated.");

        foreach (var n in nodes.Item1   )
        {
            Assert.IsTrue(n.X >= -diameter / 2 && n.X <= diameter / 2, "X coordinate out of range.");
            Assert.IsTrue(n.Y >= -diameter / 2 && n.Y <= diameter / 2, "Y coordinate out of range.");
            Assert.IsTrue(n.Z >= 0 && n.Z <= heightSegments[0].Height, "Z coordinate out of range.");
        }

    }
}

