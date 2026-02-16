namespace SD.Fem.Strand7.Services;

public class StrandApiCreateService : IStrandApiCreateService
{
    public void CreatePlates(int modelId, List<StrandPlate> plates)
    {
        var plateCount = 100;
        var seenProperties = new HashSet<int>();

        foreach (var plate in plates)
        {
            plateCount++;

            // Only set thickness once per property
            if (seenProperties.Add(plate.Poperty.PropertyNumber))
                St7.St7SetPlateThickness(modelId, plate.Poperty.PropertyNumber, [plate.Poperty.Thickness]).HandleApiError();

            var nodeCount = plate.Nodes.Count;
            var nodes = new int[St7.kMaxElementNode];
            nodes[0] = nodeCount;

            // Create all the plate nodes
            for (int i = 0; i < nodeCount; i++)
            {
                var node = plate.Nodes[i];
                if (!node.Created)
                {
                    St7.St7SetNodeXYZ(modelId, node.NodeNumber, [node.X, node.Y, node.Z]).HandleApiError();
                    node.Created = true;
                }
                nodes[i + 1] = node.NodeNumber;
            }

            // Create the plate element
            St7.St7SetElementConnection(modelId, St7.tyPLATE, plateCount, plate.Poperty.PropertyNumber, nodes).HandleApiError();
        }
    }
}