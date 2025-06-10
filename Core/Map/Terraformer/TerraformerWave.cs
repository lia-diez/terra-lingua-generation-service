using System.Numerics;
using Core.Graph;
using Core.Map.Cells;

namespace Core.Map.Terraformer;

public class TerraformerWave
{
    private readonly SpatialGraph<Cell> _map;
    private readonly Dictionary<SpatialGraphNode<Cell>, ITerraformerContext> _visitedCells;

    public IReadOnlyList<SpatialGraphNode<Cell>> Selected => _visitedCells.Keys.ToList();

    public void ClearSelection()
    {
        _visitedCells.Clear();
    }

    public TerraformerWave(SpatialGraph<Cell> map)
    {
        _map = map;
        _visitedCells = new Dictionary<SpatialGraphNode<Cell>, ITerraformerContext>();
    }


    public void SelectAndExecute(Vector2 position, ITerraformerContext context)
    {
        Select(position, context);
        Execute();
    }

    public void Select(Vector2 position, ITerraformerContext context)
    {
        var cell = _map.GetNearest(position);
        context.Node = cell;
        // var ctxInit = context.Next(cell);
        var terraformer = new TerraformerWaveWorker(this, context);
        terraformer.Execute();
    }

    public void Execute()
    {
        foreach (var ctx in _visitedCells.Values)
        {
            ctx.Execute();
        }
    }
    private class TerraformerWaveWorker
    {
        private TerraformerWave _terraformer;
        private readonly ITerraformerContext _ctx;


        public TerraformerWaveWorker(TerraformerWave terraformer, ITerraformerContext ctx)
        {
            _terraformer = terraformer;
            _ctx = ctx;
        }

        public void Execute()
        {
            _terraformer._visitedCells[_ctx.Node] = _ctx;
            if (_ctx.Priority <= 0) return;

            var neighbors = _ctx.Node.Neighbours;
            
            foreach (var neighbor in neighbors)
            {
                if (_terraformer._visitedCells.GetValueOrDefault(neighbor)?.Priority >= _ctx.Priority)
                    continue;

                var nextContext = _ctx.Next(neighbor);
                
                var newTerraformer =
                    new TerraformerWaveWorker(_terraformer, nextContext);
                newTerraformer.Execute();
            }
        }
    }
}