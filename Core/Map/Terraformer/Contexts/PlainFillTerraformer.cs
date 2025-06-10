using Core.Graph;
using Core.Map.Cells;

namespace Core.Map.Terraformer.Contexts;

public class PlainFillTerraformer : ITerraformerContext
{
    public SpatialGraphNode<Cell>? Node { get; set; } = null!;
    
    public CellType Type { get; set; }
    
    public float Priority { get; set; }

    public float Elevation { get; set; } = 0.5f;
    public virtual ITerraformerContext Next(SpatialGraphNode<Cell> next)
    {
        return new PlainFillTerraformer
        {
            Node = next,
            Type = Type,
            Priority = Priority - 1,
            Elevation = Elevation,
        };
    }

    public virtual void Execute()
    {
        if (Node?.Value == null) return;
        Node.Value.Type = Type;
        Node.Value.Elevation =  Elevation;
    }
}