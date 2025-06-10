using System.Numerics;

namespace Core.Noise;

public static class Interpolation
{
    public static bool Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c, out float u, out float v, out float w)
    {
        var v0 = b - a;
        var v1 = c - a;
        var v2 = p - a;

        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);

        float denom = d00 * d11 - d01 * d01;
        if (denom == 0)
        {
            u = v = w = 0;
            return false;
        }

        v = (d11 * d20 - d01 * d21) / denom;
        w = (d00 * d21 - d01 * d20) / denom;
        u = 1.0f - v - w;
        return u >= 0 && v >= 0 && w >= 0;
    }
    
    public static float Bilinear(float[,] noise, float x, float y)
    {
        int x0 = (int)Math.Floor(x);
        int x1 = Math.Min(x0 + 1, noise.GetLength(0) - 1);
        int y0 = (int)Math.Floor(y);
        int y1 = Math.Min(y0 + 1, noise.GetLength(1) - 1);

        float dx = x - x0;
        float dy = y - y0;

        float top = Lerp(noise[x0, y0], noise[x1, y0], dx);
        float bottom = Lerp(noise[x0, y1], noise[x1, y1], dx);

        return Lerp(top, bottom, dy);
    }

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;
}