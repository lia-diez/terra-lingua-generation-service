

using System.Numerics;
using Core.Graph;
using Core.Map.Cells;

namespace Core.Map.Terraformer;

public interface ITerraformerContext
{
    public SpatialGraphNode<Cell>? Node { get; set; }
    public float Priority { get; set; }
    public ITerraformerContext Next(SpatialGraphNode<Cell> next);
    public void Execute();
}