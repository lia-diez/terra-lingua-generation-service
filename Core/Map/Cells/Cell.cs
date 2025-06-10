using System.Numerics;

namespace Core.Map.Cells;

public class Cell(Vector2 position)
{
    public CellType Type { get; set; } = CellType.Void;
    public float Elevation { get; set; }
}