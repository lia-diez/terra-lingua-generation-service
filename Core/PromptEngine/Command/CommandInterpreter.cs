using System.Numerics;
using System.Text.Json;
using Core.Graph;
using Core.Map;
using Core.Map.Cells;
using Core.Map.Terraformer;
using Core.PromptEngine.Config;

namespace Core.PromptEngine.Command;

public static class CommandInterpreter
{
    public static void ExecuteCommands(SpatialGraph<Cell> graph, string json, MapConfig mapConfig)
    {
        var batches = JsonSerializer.Deserialize<List<CommandBatch>>(json);
        var mountainFormer = new TerraformerWave(graph);
        foreach (var batch in batches)
        {
            foreach (var cmd in batch.Commands)
            {
                switch (cmd)
                {
                    case { Name: "spawn", Arguments.Count: 5 }:
                    {
                        string cellType = cmd.Arguments[0];
                        float landRadius = float.Parse(cmd.Arguments[1]);
                        landRadius = (int)(landRadius / mapConfig.CellSize);
                        if (landRadius < 2)
                            landRadius = 2;
                        int roughness = int.Parse(cmd.Arguments[2]);
                        float landX = float.Parse(cmd.Arguments[3]);
                        float landY = float.Parse(cmd.Arguments[4]);

                        float elevation = cellType == "land" ? 0.1f : 0.0f;

                        // graph.Fill(new Vector2(landX, landY), CellType.Land, landRadius, elevation);
                        graph.FillContinent(new Vector2(landX, landY), landRadius, elevation, roughness);
                        break;
                    }
                    case { Name: "spawn_mountain", Arguments.Count: 4 }:
                        float maxHeight = float.Parse(cmd.Arguments[0]);
                        if (maxHeight > 0.5)
                            maxHeight = 0.5f;
                        if (maxHeight < 0.15)
                            maxHeight = 0.15f;
                        
                        float mountainRadius = float.Parse(cmd.Arguments[1]);
                        mountainRadius = (int)(mountainRadius / mapConfig.CellSize);
                        if (mountainRadius < 2)
                            mountainRadius = 2;
                        if (mountainRadius > 20)
                            mountainRadius = 20;
                        
                        float mountainX = float.Parse(cmd.Arguments[2]);
                        float mountainY = float.Parse(cmd.Arguments[3]);
                        mountainFormer.SpawnMountain(new Vector2(mountainX, mountainY), maxHeight, mountainRadius);
                        break;
                    default:
                        Console.WriteLine($"Unknown command or wrong arguments: {cmd.Name}");
                        break;
                }
            }
        }
        mountainFormer.Execute();
    }
}