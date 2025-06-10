namespace Core.Noise.Blends
{
    public class AddBlender : IBlender
    {
        public float Blend(float value1, float value2, float weight = 1f)
        {
            if (value1 == 0f)
                return 0f;
            float result = value1 + value2 * weight;
            result = result < 0f ? 0f : result;
            result = result > 1f ? 1f : result;
            return result;
        }
    }
}