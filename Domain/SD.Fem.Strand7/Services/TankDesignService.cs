namespace SD.Fem.Strand7.Services;

public class TankDesignService(IStrandApiCreateService strandApiCreateService,
                                     IStrandApiService strandApiService) : ITankDesignService
{
    private readonly IStrandApiCreateService _strandApiCreateService = strandApiCreateService;
    private readonly IStrandApiService _strandApiService = strandApiService;
    public async Task BuildCircularTankModel(int modelId,
                                             double diameter,
                                             List<HeightSegment> heightSegments,
                                             double meshSize,
                                             int plateNodeCount,
                                             double roofThickness,
                                             double baseThickness,
                                             string fileName)
    {
        var startPosition = new double[] { 0, 0, 0 }; // X, Y, Z coordinates for the center and base of the tank

        var circumference = Math.PI * diameter;
        var platesAround = (int)Math.Round(circumference / meshSize, MidpointRounding.AwayFromZero);
        var plateWidth = circumference / platesAround;
        var propertyNumber = 1;
        var nodeNumber = 1;

        var allPlates = new List<StrandPlate>();

        // Create base plates as a radial mesh
        if (baseThickness > 0)
        {
            var baseNodes = GenerateRadialMesh(startPosition, diameter / 2, plateWidth, ref nodeNumber);
            var basePlates = GenerateRadialPlates(baseNodes, diameter / 2, plateWidth, baseThickness, propertyNumber);
            propertyNumber++;
            nodeNumber++;
            allPlates.AddRange(basePlates);
        }

        // Take each height segment and create a cylinder for it, then combine them into a single model
        foreach (var heightSegment in heightSegments.OrderBy(hs => hs.Order))
        {
            // Calculate the number of plates needed for this height segment based on the mesh size
            var plateCount = (int)Math.Round(heightSegment.Height / meshSize, MidpointRounding.AwayFromZero);

            // Height of each plate in segment
            var plateHeight = heightSegment.Height / plateCount;

            // Create a cylinder for this height segment
            var startNode = nodeNumber;
            var nodes = GenerateCylinderNodes(startPosition, diameter / 2, heightSegment.Height, plateWidth, plateHeight, ref nodeNumber);

            var segmentPlates = Build4NodePlates(nodes.Item1, nodes.Item2, rings: plateCount, thickness: heightSegment.Thickness, propertyNumber, startNode);
            allPlates.AddRange(segmentPlates);

            propertyNumber++;
            startPosition[2] += heightSegment.Height;
        }

        // Create roof plates as a radial mesh
        if (roofThickness > 0)
        {
            nodeNumber++;
            var baseNodes = GenerateRadialMesh(startPosition, diameter / 2, plateWidth, ref nodeNumber);
            var basePlates = GenerateRadialPlates(baseNodes, diameter / 2, plateWidth, roofThickness, propertyNumber);
            allPlates.AddRange(basePlates);
        }

        _strandApiService.CreateFemModel(modelId, fileName);
        _strandApiCreateService.CreatePlates(modelId, allPlates);

        if (plateNodeCount > 4)
            _strandApiService.SubdividePlates(modelId, allPlates, plateNodeCount);

        _strandApiService.SaveAndCloseFile(modelId);
    }

    /// <summary>
    /// Generates nodes for a radial mesh.
    /// </summary>
    public static List<StrandNode> GenerateRadialMesh(double[] startPosition, double radius, double xySpacing, ref int nodeNumber)
    {
        var nodes = new List<StrandNode>
        {
            // Center node
            new()
            {
                NodeNumber = nodeNumber,
                X = startPosition[0],
                Y = startPosition[1],
                Z = startPosition[2]
            }
        };

        int numRings = (int)Math.Round(radius / xySpacing, MidpointRounding.AwayFromZero);
        double dr = radius / numRings;

        var numAngles = (int)Math.Round(2 * Math.PI * radius / xySpacing, MidpointRounding.AwayFromZero);

        for (int r = 1; r <= numRings; r++)
        {
            double currentRadius = r * dr;

            for (int a = 0; a < numAngles; a++)
            {
                double theta = 2.0 * Math.PI * a / numAngles;
                double x = currentRadius * Math.Cos(theta);
                double y = currentRadius * Math.Sin(theta);

                nodeNumber++;
                nodes.Add(new StrandNode()
                {
                    NodeNumber = nodeNumber,
                    X = startPosition[0] + x,
                    Y = startPosition[1] + y,
                    Z = startPosition[2]
                });
            }
        }

        return nodes;
    }

    public static List<StrandPlate> GenerateRadialPlates(List<StrandNode> nodes,
                                                         double radius,
                                                         double xySpacing,
                                                         double thickness,
                                                         int propertyNumber)
    {
        var plates = new List<StrandPlate>();
        var property = new StrandPlateProperty
        {
            PropertyNumber = propertyNumber,
            Thickness = thickness
        };

        // Recompute ring/angle counts from inputs
        int numRings = (int)Math.Round(radius / xySpacing, MidpointRounding.AwayFromZero);
        int numAngles = (int)Math.Round(2 * Math.PI * radius / xySpacing, MidpointRounding.AwayFromZero);

        int centerIndex = 0;
        int ringOffset = 1; // first ring starts after center

        for (int r = 1; r <= numRings; r++)
        {
            int currentRingStart = ringOffset + (r - 1) * numAngles;
            int prevRingStart = (r == 1) ? centerIndex : ringOffset + (r - 2) * numAngles;

            for (int a = 0; a < numAngles; a++)
            {
                int nextAngle = (a + 1) % numAngles;

                if (r == 1)
                {
                    // First ring: triangular plates from center
                    var center = nodes[centerIndex];
                    var n1 = nodes[currentRingStart + a];
                    var n2 = nodes[currentRingStart + nextAngle];

                    plates.Add(new StrandPlate(property, [center, n1, n2]));
                }
                else
                {
                    // Quadrilateral plates between rings
                    var inner1 = nodes[prevRingStart + a];
                    var inner2 = nodes[prevRingStart + nextAngle];
                    var outer2 = nodes[currentRingStart + nextAngle];
                    var outer1 = nodes[currentRingStart + a];

                    plates.Add(new StrandPlate(property, [inner1, inner2, outer2, outer1]));
                }
            }
        }

        return plates;
    }


    public static (List<StrandNode>, int) GenerateCylinderNodes(double[] startPosition, double radius, double height, double xySpacing, double zSpacing, ref int nodeNumber)
    {
        var nodes = new List<StrandNode>();

        // Circumference step size (angle increment)
        var dTheta = xySpacing / radius; // radians

        // Vertical step size
        var verticalSteps = (int)(height / zSpacing);

        // Count number of nodes per ring
        int nodesPerRing = 0;

        for (var k = 0; k <= verticalSteps; k++)
        {
            var z = k * zSpacing;
            for (var theta = 0D; theta < 2 * Math.PI; theta += dTheta)
            {
                if (k == 0)
                    nodesPerRing++;

                var x = radius * Math.Cos(theta);
                var y = radius * Math.Sin(theta);

                nodes.Add(new StrandNode()
                {
                    NodeNumber = nodeNumber,
                    X = startPosition[0] + x,
                    Y = startPosition[1] + y,
                    Z = startPosition[2] + z
                });
                nodeNumber++;
            }
        }

        return (nodes, nodesPerRing);
    }
    public static List<StrandPlate> Build4NodePlates(List<StrandNode> nodes,
                                                     int nodesPerRing,
                                                     int rings,
                                                     double thickness,
                                                     int propertyNumber,
                                                     int startNodeNumber)
    {
        var plates = new List<StrandPlate>();
        var property = new StrandPlateProperty
        {
            PropertyNumber = propertyNumber,
            Thickness = thickness
        };

        // Loop through each vertical segment (between two rings)
        for (int k = 0; k < rings; k++)
        {
            int ringStart = k * nodesPerRing + startNodeNumber - 1;
            int nextRingStart = (k + 1) * nodesPerRing + startNodeNumber - 1;

            // Loop around the circumference
            for (int i = 1; i <= nodesPerRing; i++)
            {
                // Four nodes of the quadrilateral plate
                // bottom-left
                var n1 = nodes.First(n => n.NodeNumber == ringStart + i);

                // bottom-right
                var secondNode = ringStart + i + 1;
                if (i == nodesPerRing)
                    secondNode = ringStart + 1;

                var n2 = nodes.First(n => n.NodeNumber == secondNode);

                // top-right
                var fourthNode = nextRingStart + i + 1;
                if (i == nodesPerRing)
                    fourthNode = nextRingStart + 1;

                var n3 = nodes.First(n => n.NodeNumber == fourthNode);

                // top-left
                var n4 = nodes.First(n => n.NodeNumber == nextRingStart + i);

                plates.Add(new StrandPlate(property, nodes: [n1, n2, n3, n4]));
            }
        }

        return plates;
    }
}
