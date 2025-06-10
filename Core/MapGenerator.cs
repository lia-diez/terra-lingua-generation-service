using System.Numerics;
using Cli.Helpers;
using Core.Drawing;
using Core.Graph;
using Core.Graph.Points;
using Core.Map;
using Core.Map.Cells;
using Core.Map.Terraformer;
using Core.Noise;
using Core.Noise.Blends;
using Core.PromptEngine.Command;
using Core.PromptEngine.Config;

namespace Core;

public class MapGenerator
{
    public static Canvas GenerateMap(string json)
    {
        MapConfig mapConfig = new MapConfig()
        {
            Width = 512,
            Height = 512,
            Resolution = 64f,
            DisplaceStrength = 0.4f,
            Seed = TimeBasedRandom.Generate()
        };
        
        GlobalRandom.Seed = mapConfig.Seed;
        
        var image = new Canvas(mapConfig.Width, mapConfig.Height);
        image.Fill(Color.White);

        var pointsProvider = new UniformPointProvider(mapConfig.DisplaceStrength, 1f / mapConfig.Resolution);
        var positions = pointsProvider.GetPoints(new Vector2(mapConfig.Width, mapConfig.Height));
        var map = new SpatialGraph<Cell>(new Vector2(mapConfig.Width, mapConfig.Height));

        foreach (var p in positions)
        {
            var cell = new Cell(p)
            {
                Type = CellType.Water,
            };
            map.AddNode(new SpatialGraphNode<Cell>(p, cell));
        }
        
        map.ConcurrentAutoLink(new Vector2(0.9f, 0.9f));

        CommandInterpreter.ExecuteCommands(map, json, mapConfig);

        image.DrawSmoothMap(map);
        image.GaussianBlur();

        var diamond1 = CanvasDrawers.GenerateDiamondSquare(image.Width, image.Height, image.Width / 16 + 1,0.7f);
        var diamond2 = CanvasDrawers.GenerateDiamondSquare(image.Width, image.Height, image.Width / 8 + 1,0.8f);
        SoftBlender sblender = new SoftBlender();
        image.Blend(diamond1, sblender, 0.9f);
        image.Blend(diamond2, sblender, 0.9f);
        
        return image;
    }
}