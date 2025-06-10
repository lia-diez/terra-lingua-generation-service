namespace Core.PromptEngine.Config;

public class MapConfig
{
    public int Width { get; set; }
    public int Height { get; set; }
    public float Resolution { get; set; }
    public float DisplaceStrength { get; set; } = 1.0f;
    public int Seed { get; set; }
    
    public int CellSize => (int)Math.Sqrt(Resolution);

}