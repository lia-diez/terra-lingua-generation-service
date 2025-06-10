namespace Core.Noise.Blends;

public class SoftBlender : IBlender
{
    public float Blend(float value1, float value2, float weight = 1f)
    {
        float result;

        if (value2 < 0.5f)
            result = value1 - (1 - 2 * value2) * value1 * (1 - value1);
        else
            result = value1 + (2 * value2 - 1) * (MathF.Sqrt(value1) - value1);

        return value1 + (result - value1) * weight;
    }
}