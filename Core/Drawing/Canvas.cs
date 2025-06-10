using System.Numerics;

namespace Core.Drawing;

public class Canvas
{
    public IReadOnlyList<Vector3> GetInternalArray => _pixels;
    public Vector3[] GetInternalArrayUnsafe => _pixels;
    private Vector3[] _pixels;

    public int Width { get; }
    public int Height { get; }
    public int Size => Width * Height;
    
    public Vector3 this[int x, int y]
    {
        get => _pixels[y * Width + x];
        set => _pixels[y * Width + x] = value;
    }

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        _pixels = new Vector3[width * height];
        Fill(Color.Black);
    }

    public void SetPixel(int x, int y, Vector3 color)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds.");

        _pixels[y * Width + x] = color;
    }

    public void SetPixelUnsafe(int x, int y, Vector3 color)
    {
        _pixels[y * Width + x] = color;
    }

    public Vector3 GetPixel(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds.");

        return _pixels[y * Width + x];
    }
    
    public float GetPixelFloat(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException($"Coordinates ({x}, {y}) are out of bounds.");

        return _pixels[y * Width + x].X;
    }

    public Vector3 GetPixelUnsafe(int x, int y)
    {
        return _pixels[y * Width + x];
    }

    public void Fill(Vector3 color)
    {
        for (int i = 0; i < _pixels.Length; i++)
        {
            _pixels[i] = color;
        }
    }
    
    public void DrawLine(int x1, int y1, int x2, int y2, Vector3 color)
    {
        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            SetPixel(x1, y1, color);

            if (x1 == x2 && y1 == y2)
                break;

            int err2 = err * 2;
            if (err2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }

            if (err2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }
    
    public void DrawRectangle(int x, int y, int width, int height, Vector3 color)
    {
        for (int i = 0; i < width; i++)
        {
            SetPixel(x + i, y, color);
            SetPixel(x + i, y + height - 1, color);
        }

        for (int i = 0; i < height; i++)
        {
            SetPixel(x, y + i, color);
            SetPixel(x + width - 1, y + i, color);
        }
    }
    
    public void DrawCircle(int centerX, int centerY, int radius, Vector3 color)
    {
        int x = radius;
        int y = 0;
        int err = 0;
        while (x >= y)
        {
            SetPixel(centerX + x, centerY + y, color);
            SetPixel(centerX + y, centerY + x, color);
            SetPixel(centerX - y, centerY + x, color);
            SetPixel(centerX - x, centerY + y, color);
            SetPixel(centerX - x, centerY - y, color);
            SetPixel(centerX - y, centerY - x, color);
            SetPixel(centerX + y, centerY - x, color);
            SetPixel(centerX + x, centerY - y, color);

            if (err <= 0)
            {
                err += 2 * ++y + 1;
            }

            if (err > 0)
            {
                err -= 2 * --x;
            }
        }
    }
    
    public void FillCircle(int centerX, int centerY, int radius, Vector3 color)
    {
        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    if (centerX + x < 0 || centerX + x >= Width || centerY + y < 0 || centerY + y >= Height)
                        continue;
                    SetPixel(centerX + x, centerY + y, color);
                }
            }
        }
    }
}