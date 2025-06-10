namespace Core.PromptEngine.Command;

using System.Text.Json.Serialization;

public class Command
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("arguments")] public List<string> Arguments { get; set; }
}

public class CommandBatch
{
    [JsonPropertyName("commands")] public List<Command> Commands { get; set; }
}