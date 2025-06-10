namespace Core.Noise.Generators;

public static class PerlinGenerator
{
    private static readonly int[] Permutation = Enumerable.Range(0, 256).ToArray();
    private static readonly int[] P;

    static PerlinGenerator()
    {
        var rng = GlobalRandom.Instance;
        Permutation = Permutation.OrderBy(_ => rng.Next()).ToArray();
        P = new int[512];
        for (int i = 0; i < 512; i++)
            P[i] = Permutation[i % 256];
    }

    private static float Fade(float t) =>
        t * t * t * (t * (t * 6 - 15) + 10);

    private static float Lerp(float a, float b, float t) =>
        a + t * (b - a);

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 7;
        float u = h < 4 ? x : y;
        float v = h < 4 ? y : x;
        return ((h & 1) == 0 ? u : -u) +
               ((h & 2) == 0 ? v : -v);
    }

    public static float Perlin(float x, float y)
    {
        int xi = (int)MathF.Floor(x) & 255;
        int yi = (int)MathF.Floor(y) & 255;
        float xf = x - MathF.Floor(x);
        float yf = y - MathF.Floor(y);

        float u = Fade(xf);
        float v = Fade(yf);

        int aa = P[P[xi] + yi];
        int ab = P[P[xi] + yi + 1];
        int ba = P[P[xi + 1] + yi];
        int bb = P[P[xi + 1] + yi + 1];

        float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
        return (Lerp(x1, x2, v) + 1) / 2; // Normalize to [0,1]
    }
}