namespace Core.Noise.Blends;

public interface IBlender
{
    public float Blend(float value1, float value2, float weight);
}