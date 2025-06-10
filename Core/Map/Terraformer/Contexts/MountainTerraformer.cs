using Core.Graph;
using Core.Map.Cells;

namespace Core.Map.Terraformer.Contexts;

public class MountainTerraformer : ITerraformerContext
{
    public SpatialGraphNode<Cell>? Node { get; set; }
    public float Priority { get; set; }

    public float Slopness { get; set; }
    public readonly float Precision = 1 / 255f;


    public ITerraformerContext Next(SpatialGraphNode<Cell> next)
    {
        var priority = Priority * Slopness;
        priority = priority < Precision ? 0 : priority;
        return new MountainTerraformer
        {
            Node = next,
            Priority = priority,
            Slopness = Slopness,
        };
    }

    public void Execute()
    {
        if (Node?.Value == null)
            return; 
        if (Node.Value.Type == CellType.Water)
            return;
        Node.Value.Elevation += Priority;
    }
}