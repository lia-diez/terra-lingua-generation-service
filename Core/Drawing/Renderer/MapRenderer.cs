using System.Numerics;
using Core.Graph;
using Core.Map.Cells;

namespace Core.Drawing.Renderer;

public static class MapRenderer
{
    public static void DrawMapV1(this Canvas canvas, SpatialGraph<Cell> graph)
    {
        var xScale = canvas.Width / graph.Size.X;
        var yScale = canvas.Height / graph.Size.Y;


        for (int i = 0; i < canvas.Width; i++)
        {
            for (int j = 0; j < canvas.Height; j++)
            {
                var node = graph.GetNearest(new Vector2(i / xScale, j / yScale));
                var color = colorMap[node.Value?.Type ?? CellType.Void];
                canvas.SetPixel(i, j, color);
            }
        }
    }
    
    // normalizes the elevation of the graph to be between 0 and 1
    public static void NormalizeElevation(this SpatialGraph<Cell> graph)
    {
        var min = float.MaxValue;
        var max = float.MinValue;
        foreach (var node in graph.NodesList)
        {
            if (node.Value == null)
                continue;
            if (node.Value.Elevation < min)
                min = node.Value.Elevation;
            if (node.Value.Elevation > max)
                max = node.Value.Elevation;
        }

        foreach (var node in graph.NodesList)
        {
            if (node.Value == null)
                continue;
            node.Value.Elevation = (node.Value.Elevation - min) / (max - min);
        }
    }

    private static readonly Dictionary<CellType, Vector3> colorMap = new()
    {
        { CellType.Land, Color.FromHex("#dbcd95") },
        { CellType.Water, Color.FromHex("#95b9db") },
        { CellType.Void, Color.FromHex("#2d2f30") },
        { CellType.Marker, Color.FromHex("#ff0000") },
    };
}