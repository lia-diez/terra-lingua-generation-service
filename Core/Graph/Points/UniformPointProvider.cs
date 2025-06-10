using System.Numerics;
using Core.Noise;

namespace Core.Graph.Points;

public class UniformPointProvider(float entropy, float density) : IPointProvider
{
    public Vector2[] GetPoints(Vector2 size)
    {
        float step = (float)(1 / Math.Sqrt(density));
        int estimatedCount = (int)((size.X / step) * (size.Y / step));
        var points = new Vector2[estimatedCount];

        int k = 0;
        for (float y = step / 2; y < size.Y; y += step)
        {
            for (float x = step / 2; x < size.X; x += step)
            {
                if (k >= estimatedCount)
                    break;
                
                float dx = Clamp(Displacer.DisplaceValue(x, step, entropy), 0, size.X - 1);
                float dy = Clamp(Displacer.DisplaceValue(y, step, entropy), 0, size.Y - 1);

                points[k++] = new Vector2(dx, dy);
            }
        }

        if (k < estimatedCount)
            Array.Resize(ref points, k); // Trim excess if fewer points

        return points;
    }

    private float Clamp(float val, float min, float max)
    {
        return val < min ? min : val > max ? max : val;
    }
}