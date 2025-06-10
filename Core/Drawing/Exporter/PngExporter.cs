using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Core.Drawing.Exporter;

public class PngCanvasExporter : ICanvasExporter
{
    public void Export(string filePath, Canvas canvas)
    {
        using var image = new Image<Rgb24>(canvas.Width, canvas.Height);

        for (int y = 0; y < canvas.Height; y++)
        {
            for (int x = 0; x < canvas.Width; x++)
            {
                var vec = canvas.GetPixelUnsafe(x, y);
                image[x, y] = new Rgb24(
                    ToByte(vec.X),
                    ToByte(vec.Y),
                    ToByte(vec.Z)
                );
            }
        }

        image.Save(filePath, new PngEncoder());
    }

    private static byte ToByte(float value)
    {
        return (byte)Math.Clamp(value * 255f, 0, 255);
    }
}