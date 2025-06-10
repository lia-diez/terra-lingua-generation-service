using System.Numerics;

namespace Core.Drawing;

public static class Color
{
    public static Vector3 Black => new Vector3(0, 0, 0);
    public static Vector3 White => new Vector3(1, 1, 1);
    public static Vector3 Red => new Vector3(1, 0, 0);
    public static Vector3 Green => new Vector3(0, 1, 0);
    public static Vector3 Blue => new Vector3(0, 0, 1);
    public static Vector3 Yellow => new Vector3(1, 1, 0);
    public static Vector3 Cyan => new Vector3(0, 1, 1);
    public static Vector3 Magenta => new Vector3(1, 0, 1);
    public static Vector3 Gray => new Vector3(0.5f, 0.5f, 0.5f);
    public static Vector3 LightGray => new Vector3(0.75f, 0.75f, 0.75f);
    public static Vector3 DarkGray => new Vector3(0.25f, 0.25f, 0.25f);
    public static Vector3 FromRgb(int r, int g, int b)
    {
        return new Vector3(r / 255f, g / 255f, b / 255f);
    }

    public static Vector3 FromHex(string hex)
    {
        if (hex.Length != 7 || hex[0] != '#')
            throw new ArgumentException("Hex color must be in the format #RRGGBB");

        int r = Convert.ToInt32(hex.Substring(1, 2), 16);
        int g = Convert.ToInt32(hex.Substring(3, 2), 16);
        int b = Convert.ToInt32(hex.Substring(5, 2), 16);

        return FromRgb(r, g, b);
    }
}