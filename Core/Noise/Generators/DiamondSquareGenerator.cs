namespace Core.Noise.Generators;

public class DiamondSquareGenerator
{
    private Random random = GlobalRandom.Instance;
    private float roughness;
    private int size;
    private float[,] heightMap;

    /// <summary>
    /// Creates a new Diamond-Square terrain generator.
    /// </summary>
    /// <param name="size">Size of the terrain (must be 2^n + 1)</param>
    /// <param name="roughness">Controls the roughness of the terrain. Recommended range: 0.3f-1.0f</param>
    /// <param name="seed">Random seed for reproducible results</param>
    public DiamondSquareGenerator(int size, float roughness)
    {
        this.size = size;
        this.roughness = roughness;
        this.heightMap = new float[size, size];
    }

    /// <summary>
    /// Generates a heightmap using the Diamond-Square algorithm.
    /// </summary>
    /// <param name="minHeight">Minimum height value</param>
    /// <param name="maxHeight">Maximum height value</param>
    /// <returns>2D array of height values</returns>
    public float[,] Generate(float minHeight = 0.0f, float maxHeight = 1.0f)
    {
        // Initialize corner values with random heights
        heightMap[0, 0] = RandomHeight(1.0f);
        heightMap[0, size - 1] = RandomHeight(1.0f);
        heightMap[size - 1, 0] = RandomHeight(1.0f);
        heightMap[size - 1, size - 1] = RandomHeight(1.0f);

        // Start the recursive algorithm
        DiamondSquareAlgorithm(size - 1, 1.0f);

        // Normalize height values to the desired range
        NormalizeHeightMap(minHeight, maxHeight);

        return heightMap;
    }

    private void DiamondSquareAlgorithm(int stepSize, float scale)
    {
        int halfStep = stepSize / 2;

        if (halfStep < 1)
            return;

        // Diamond step
        for (int y = halfStep; y < size; y += stepSize)
        {
            for (int x = halfStep; x < size; x += stepSize)
            {
                DiamondStep(x, y, halfStep, scale);
            }
        }

        // Square step
        for (int y = 0; y < size; y += halfStep)
        {
            for (int x = (y + halfStep) % stepSize; x < size; x += stepSize)
            {
                SquareStep(x, y, halfStep, scale);
            }
        }

        // Recursively process the next level with reduced randomness
        DiamondSquareAlgorithm(halfStep, scale * roughness);
    }

    private void DiamondStep(int x, int y, int halfStep, float scale)
    {
        // Average the four corners
        float avg = (
            heightMap[y - halfStep, x - halfStep] + // top-left
            heightMap[y - halfStep, x + halfStep] + // top-right
            heightMap[y + halfStep, x - halfStep] + // bottom-left
            heightMap[y + halfStep, x + halfStep] // bottom-right
        ) / 4.0f;

        // Add random displacement
        heightMap[y, x] = avg + RandomHeight(scale);
    }

    private void SquareStep(int x, int y, int halfStep, float scale)
    {
        int count = 0;
        float sum = 0.0f;

        // Check all four directions and add values if they're in bounds
        if (y - halfStep >= 0)
        {
            sum += heightMap[y - halfStep, x];
            count++;
        }

        if (y + halfStep < size)
        {
            sum += heightMap[y + halfStep, x];
            count++;
        }

        if (x - halfStep >= 0)
        {
            sum += heightMap[y, x - halfStep];
            count++;
        }

        if (x + halfStep < size)
        {
            sum += heightMap[y, x + halfStep];
            count++;
        }


        // Calculate average and add random displacement
        float avg = sum / count;
        heightMap[y, x] = avg + RandomHeight(scale);
    }

    private float RandomHeight(float scale)
    {
        return (float)(random.NextDouble() * 2 - 1) * scale;
    }

    private void NormalizeHeightMap(float minHeight, float maxHeight)
    {
        // Find current min and max
        float currentMin = float.MaxValue;
        float currentMax = float.MinValue;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                currentMin = Math.Min(currentMin, heightMap[y, x]);
                currentMax = Math.Max(currentMax, heightMap[y, x]);
            }
        }

        // Normalize to [0, 1] then scale to [minHeight, maxHeight]
        float range = currentMax - currentMin;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Normalize to [0, 1]
                float normalizedHeight = (heightMap[y, x] - currentMin) / range;

                // Scale to [minHeight, maxHeight]
                heightMap[y, x] = minHeight + normalizedHeight * (maxHeight - minHeight);
            }
        }
    }
}