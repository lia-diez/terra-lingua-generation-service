using Core.Graph;
using Core.Map.Cells;

namespace Core.Map.Terraformer.Contexts;

public class SelectionPlainFillTerraformer : PlainFillTerraformer
{
    public List<SpatialGraphNode<Cell>?> Selected { get; set; } = new();
    
    public override void Execute()
    {
        if(!Selected.Contains(Node)) return;
        base.Execute();
    }
}