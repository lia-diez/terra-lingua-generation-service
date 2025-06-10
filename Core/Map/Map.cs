using System.Numerics;
using Core.Graph;
using Core.Map.Cells;
using Core.Map.Terraformer;
using Core.Map.Terraformer.Contexts;
using Core.Noise;

namespace Core.Map;

public static class Map
{
    public static IReadOnlyList<SpatialGraphNode<Cell>> Fill(this SpatialGraph<Cell> graph, Vector2 position,
        CellType type, float radius,
        float elevation)
    {
        var tformer = new TerraformerWave(graph);
        var ctx = new PlainFillTerraformer
        {
            Type = type,
            Priority = radius,
            Elevation = elevation
        };
        tformer.SelectAndExecute(position, ctx);
        return tformer.Selected;
    }

    public static IReadOnlyList<SpatialGraphNode<Cell>> SafeFill(this SpatialGraph<Cell> graph,
        List<SpatialGraphNode<Cell>> selection, Vector2 position, CellType type, float radius, float elevation)
    {
        var tformer = new TerraformerWave(graph);
        var ctx = new SelectionPlainFillTerraformer()
        {
            Type = type,
            Priority = radius,
            Elevation = elevation,
            Selected = selection
        };
        tformer.SelectAndExecute(position, ctx);
        return tformer.Selected;
    }

    public static void FillContinent(this SpatialGraph<Cell> graph, Vector2 position, float radius,
        float elevation, int roughness)
    {
        var continent = graph.Fill(position, CellType.Land, radius, elevation);
        var edge = continent.Where(n => n.Neighbours.Any(nn => nn.Value?.Type != n.Value?.Type)).ToList();

        var samples = new List<SpatialGraphNode<Cell>>();
        for (int i = 0; i < roughness; i++)
        {
            var random = GlobalRandom.Instance.Next(0, edge.Count);
            samples.Add(edge.ElementAt(random));
        }


        var craters = new List<List<SpatialGraphNode<Cell>>>();
        foreach (var sample in samples)
        {
            var crater = graph.SafeFill(continent.ToList(), sample.Position, CellType.Water, radius / 2, 0);
            craters.Add(crater.ToList());
        }

        var allCraters = craters.SelectMany(c => c).ToList();

        var cratersEdges = allCraters.Where(n => n.Neighbours.Any(nn => nn.Value?.Type != n.Value?.Type)).ToList();

        var cratersSamples = new List<SpatialGraphNode<Cell>>();

        for (int i = 0; i < roughness * 2; i++)
        {
            var random = GlobalRandom.Instance.Next(0, cratersEdges.Count);
            cratersSamples.Add(cratersEdges.ElementAt(random));
        }

        foreach (var sample in cratersSamples)
        {
            graph.SafeFill(allCraters.ToList(), sample.Position, CellType.Land, radius / 8, elevation);
        }
    }
    
    public static void SpawnMountain(this SpatialGraph<Cell> graph, Vector2 position, float height, float radius)
    {
        var tformer = new TerraformerWave(graph);
        SpawnMountain(tformer, position, height, radius);
        tformer.Execute();
    }
    
    public static void SpawnMountain(this TerraformerWave tformer, Vector2 position, float height, float radius)
    {
        var steepness = float.Pow((1 / 255f) / height, 1 / (radius - 1));
        var ctx = new MountainTerraformer
        {
            Priority = height,
            Slopness = steepness
        };
        tformer.Select(position, ctx);
    }

    public static void SpawnMountainRange(this SpatialGraph<Cell> graph, Vector2 start, Vector2 end, float height, float radius,
        int count, float heightEntropy, float positionEntropy, float radiusEntropy)
    {
        var tformer = new TerraformerWave(graph);
        
        List<(Vector2 position, float height, float radius)> mountains = new();
        Vector2 delta = (end - start) / count;
        Vector2 currentPosition = start;

        for (int i = 0; i < count; i++)
        {
            var displacedPosition = Displacer.DisplaceVector(currentPosition, delta, positionEntropy);
            mountains.Add((
                    displacedPosition,
                Displacer.DisplaceValue(height, height, heightEntropy),
                Displacer.DisplaceValue(radius, height, radiusEntropy)
                ));
            currentPosition += delta;
        }
        
        foreach (var mountain in mountains)
        {
            SpawnMountain(tformer, mountain.position, mountain.height, mountain.radius);
        }
        
        tformer.Execute();
    }
}