using System.Numerics;

namespace Core.Noise;

public static class Displacer
{
    public static float DisplaceValue(float value, float step,float entropy)
    {
        return value + (float)(GlobalRandom.Instance.NextDouble() * 2 - 1) * step * entropy;
    }
    
    public static float DisplaceValue(float value, float entropy)
    {
        return value + (float)(GlobalRandom.Instance.NextDouble() * 2 - 1) * entropy;
    }

    public static Vector2 DisplaceVector(Vector2 value, Vector2 step, float entropy)
    {
        return new Vector2(
            DisplaceValue(value.X, step.X, entropy),
            DisplaceValue(value.Y, step.Y, entropy)
        );
    }
}