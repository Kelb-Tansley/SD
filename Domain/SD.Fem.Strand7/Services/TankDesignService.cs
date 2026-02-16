namespace SD.Fem.Strand7.Services;

public class TankDesignService(IStrandApiCreateService strandApiCreateService,
                                     IStrandApiService strandApiService) : ITankDesignService
{
    private readonly IStrandApiCreateService _strandApiCreateService = strandApiCreateService;
    private readonly IStrandApiService _strandApiService = strandApiService;
    public async Task BuildCircularTankModel(int modelId, double diameter, List<HeightSegment> heightSegments, double meshSize, int plateNodeCount, double roofThickness, double baseThickness, string fileName)
    {
        var startPosition = new double[] { 0, 0, 0 }; // X, Y, Z coordinates for the center and base of the tank

        var circumference = Math.PI * diameter;
        var platesAround = (int)Math.Round(circumference / meshSize, MidpointRounding.AwayFromZero);
        var plateWidth = circumference / platesAround;
        var propertyNumber = 1;
        var nodeNumber = 1;

        var allPlates = new List<StrandPlate>();
        // Take each height segment and create a cylinder for it, then combine them into a single model
        foreach (var heightSegment in heightSegments)
        {
            // Calculate the number of plates needed for this height segment based on the mesh size
            var plateCount = (int)Math.Round(heightSegment.Height / meshSize, MidpointRounding.AwayFromZero);

            // Height of each plate in segment
            var plateHeight = heightSegment.Height / plateCount;

            // Create a cylinder for this height segment
            if (plateNodeCount == 4)
            {
                var startNode = nodeNumber;
                var nodes = GenerateCylinderNodes(startPosition, diameter / 2, heightSegment.Height, meshSize, plateHeight, ref nodeNumber);
                allPlates.AddRange(Build4NodePlates(nodes.Item1, nodes.Item2, rings: plateCount, thickness: heightSegment.Thickness, propertyNumber, startNode));
            }

            propertyNumber++;
        }

        _strandApiService.CreateFemModel(modelId, fileName);
        _strandApiCreateService.CreatePlates(modelId, allPlates);
        _strandApiService.SaveAndCloseFile(modelId);

    }

    private static StrandPlate Create4NodePlate(double diameter, double[] startPosition, double plateWidth, int propertyNumber, HeightSegment heightSegment, double plateHeight, ref int nodeNumber, ref double deltaX, ref double deltaY)
    {
        var nodes = new List<StrandNode>();

        // Node 1
        nodes.Add(new StrandNode()
        {
            NodeNumber = nodeNumber,
            X = startPosition[0] + diameter / 2 - deltaX,
            Y = startPosition[1],
            Z = startPosition[2]
        });
        nodeNumber++;

        // Node 2
        deltaX += plateWidth * Math.Sin(plateWidth / (diameter / 2));
        deltaY += Math.Sqrt(Math.Pow(plateWidth, 2) - Math.Pow(deltaX, 2));
        nodes.Add(new StrandNode()
        {
            NodeNumber = nodeNumber,
            X = startPosition[0] + diameter / 2 - deltaX,
            Y = startPosition[1] + deltaY,
            Z = startPosition[2]
        });
        nodeNumber++;

        // Node 3
        nodes.Add(new StrandNode()
        {
            NodeNumber = nodeNumber,
            X = startPosition[0] + diameter / 2,
            Y = startPosition[1],
            Z = startPosition[2] + plateHeight
        });
        nodeNumber++;

        // Node 4
        nodes.Add(new StrandNode()
        {
            NodeNumber = nodeNumber,
            X = startPosition[0] + diameter / 2 - deltaX,
            Y = startPosition[1] + deltaY,
            Z = startPosition[2] + plateHeight
        });
        nodeNumber++;

        var property = new StrandPlateProperty() { Thickness = heightSegment.Thickness, PropertyNumber = propertyNumber };
        var newPlate = new StrandPlate(property, nodes);
        return newPlate;
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
            int ringStart = k * nodesPerRing + startNodeNumber -1;
            int nextRingStart = (k + 1) * nodesPerRing + startNodeNumber -1;

            // Loop around the circumference
            for (int i = 1; i <= nodesPerRing; i++)
            {
                // Four nodes of the quadrilateral plate
                var n1 = nodes.First(n => n.NodeNumber == ringStart + i);       // bottom-left

                // bottom-right
                var secondNode = ringStart + i + 1;
                if (i == nodesPerRing)
                    secondNode = ringStart + 1;

                var n2 = nodes.First(n => n.NodeNumber == secondNode);
                var n3 = nodes.First(n => n.NodeNumber == nextRingStart + i);   // top-left

                // top-right
                var fourthNode = nextRingStart + i + 1;
                if (i == nodesPerRing)
                    fourthNode = nextRingStart + 1;

                var n4 = nodes.First(n => n.NodeNumber == fourthNode);

                plates.Add(new StrandPlate(property, nodes: [n1, n2, n3, n4]));
            }
        }

        return plates;
    }
}
