using System.Numerics;
using Core.Graph;
using Core.Map.Cells;
using Core.Noise;
using Core.Noise.Blends;
using Core.Noise.Generators;

namespace Core.Drawing;

public static class CanvasDrawers
{
    public static void DrawGraph<T>(this Canvas canvas, SpatialGraph<T> graph, Vector3 nodeColor,
        Vector3 edgeColor, int nodeRadius = 3)
    {
        var xScale = canvas.Width / graph.Size.X;
        var yScale = canvas.Height / graph.Size.Y;

        foreach (var node in graph.NodesList)
        {
            foreach (var neighbour in node.Neighbours)
            {
                var x1 = (int)(node.Position.X * xScale);
                var y1 = (int)(node.Position.Y * yScale);
                var x2 = (int)(neighbour.Position.X * xScale);
                var y2 = (int)(neighbour.Position.Y * yScale);
                canvas.DrawLine(x1, y1, x2, y2, edgeColor);
            }
        }

        foreach (var node in graph.NodesList)
        {
            var x = (int)(node.Position.X * xScale);
            var y = (int)(node.Position.Y * yScale);
            canvas.FillCircle(x, y, nodeRadius, nodeColor);
        }
    }

    public static void DrawMap(this Canvas canvas, SpatialGraph<Cell> graph)
    {
        var xScale = canvas.Width / graph.Size.X;
        var yScale = canvas.Height / graph.Size.Y;

        Parallel.For(0, canvas.Width, i =>
        {
            for (int j = 0; j < canvas.Height; j++)
            {
                var node = graph.GetNearest(new Vector2(i / xScale, j / yScale));
                // var color = colorMap[node.Value?.Type ?? CellType.Void];
                var elevation = (node.Value?.Elevation ?? 0);
                var color = new Vector3(elevation, elevation, elevation);
                canvas.SetPixel(i, j, color);
            }
        });
    }

    #region Smooth Map (Barycentric Interpolation)

    public static void DrawSmoothMap(this Canvas canvas, SpatialGraph<Cell> graph)
    {
        var xScale = canvas.Width / graph.Size.X;
        var yScale = canvas.Height / graph.Size.Y;

        Parallel.For(0, canvas.Width, i =>
        {
            for (int j = 0; j < canvas.Height; j++)
            {
                var pos = new Vector2(i / xScale, j / yScale);
                var center = graph.GetNearest(pos);
                var neighbors = center.Neighbours.ToList();
                float elevation = center.Value?.Elevation ?? 0;

                bool found = false;

                elevation = BarycentricInterpolation(neighbors, found, pos, center, elevation);

                var color = new Vector3(elevation, elevation, elevation);
                canvas.SetPixel(i, j, color);
            }
        });
    }

    private static float BarycentricInterpolation(List<SpatialGraphNode<Cell>> neighbors, bool found, Vector2 pos,
        SpatialGraphNode<Cell> center,
        float elevation)
    {
        for (int a = 0; a < neighbors.Count && !found; a++)
        {
            for (int b = a + 1; b < neighbors.Count && !found; b++)
            {
                var n1 = neighbors[a];
                var n2 = neighbors[b];

                if (Interpolation.Barycentric(pos, center.Position, n1.Position, n2.Position, out float u, out float v,
                        out float w))
                {
                    float e0 = center.Value?.Elevation ?? 0;
                    float e1 = n1.Value?.Elevation ?? 0;
                    float e2 = n2.Value?.Elevation ?? 0;

                    elevation = u * e0 + v * e1 + w * e2;
                    found = true;
                }
            }
        }

        return elevation;
    }

    #endregion

    #region Smooth Map (Gaussian Blur)

    public static void GaussianBlur(this Canvas canvas, int kernelSize = 5,
        float sigma = 1.0f)
    {
        var width = canvas.Width;
        var height = canvas.Height;

        float[,] elevationMap = new float[width, height];
        float[,] blurredMap = new float[width, height];

        // Step 1: Sample elevations
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // elevation is in green channel
                elevationMap[i, j] = canvas.GetPixel(i, j).Y;
            }
        }

        float[,] kernel = GenerateGaussianKernel(kernelSize, sigma);

        int radius = kernelSize / 2;

        // Step 3: Apply Gaussian blur
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float sum = 0;
                float weightSum = 0;

                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        int x = i + dx;
                        int y = j + dy;

                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            float weight = kernel[dx + radius, dy + radius];
                            sum += elevationMap[x, y] * weight;
                            weightSum += weight;
                        }
                    }
                }

                blurredMap[i, j] = sum / weightSum;
            }
        }

        // Step 4: Draw blurred elevation map
        Parallel.For(0, width, i =>
        {
            for (int j = 0; j < height; j++)
            {
                var elevation = blurredMap[i, j];
                var color = new Vector3(elevation, elevation, elevation);
                canvas.SetPixel(i, j, color);
            }
        });
    }

    private static float[,] GenerateGaussianKernel(int size, float sigma)
    {
        float[,] kernel = new float[size, size];
        int radius = size / 2;
        float twoSigmaSq = 2 * sigma * sigma;
        float piSigma = (float)(2 * Math.PI * sigma * sigma);
        float sum = 0;

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                float value = (float)Math.Exp(-(x * x + y * y) / twoSigmaSq) / piSigma;
                kernel[x + radius, y + radius] = value;
                sum += value;
            }
        }

        // Normalize the kernel
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                kernel[i, j] /= sum;
            }
        }

        return kernel;
    }

    #endregion

    #region Noises

    public static Canvas GeneratePerlin(int width, int height, float scale = 0.1f)
    {
        var canvas = new Canvas(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = PerlinGenerator.Perlin(x * scale, y * scale);
                canvas.SetPixel(x, y, new Vector3(value));
            }
        }

        return canvas;
    }
    
    public static Canvas GenerateSimplex(int width, int height, float scale = 0.1f)
    {
        var canvas = new Canvas(width, height);
        var simplex = new SimplexGenerator();
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = simplex.Noise(x, y);
                value = (value + 1) / 2; // Normalize to [0, 1]
                canvas.SetPixel(x, y, new Vector3(value));
            }
        }
        
        return canvas;
    }

    public static Canvas GenerateDiamondSquare(int width, int height, int noiseSize, float roughness)
    {
        float scaleX = (float)(noiseSize - 1) / width;
        float scaleY = (float)(noiseSize - 1) / height;

        var canvas = new Canvas(width, height);
        var heightmap = new DiamondSquareGenerator(noiseSize, roughness).Generate();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x * scaleX;
                float sampleY = y * scaleY;

                float value = Interpolation.Bilinear(heightmap, sampleX, sampleY);
                canvas.SetPixel(x, y, new Vector3(value));
            }
        }
        return canvas;
    }

    #endregion

    #region Blend

    public static void Blend(this Canvas canvas, Canvas otherCanvas, IBlender blender, float alpha)
    {
        for (int i = 0; i < canvas.Width; i++)
        {
            for (int j = 0; j < canvas.Height; j++)
            {
                var color1 = canvas.GetPixelFloat(i, j);
                var color2 = otherCanvas.GetPixelFloat(i, j);
                var blendedColor = blender.Blend(color1, color2, alpha);
                canvas.SetPixel(i, j, new Vector3(blendedColor));
            }
        }
    }

    #endregion
}