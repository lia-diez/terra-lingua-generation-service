using System.Numerics;

namespace Core.Graph.Points;

public interface IPointProvider
{
    public Vector2[] GetPoints(Vector2 size);
}