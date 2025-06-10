using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Core.Drawing.Exporter;

public class PngStreamCanvasExporter
{
    public byte[] Export(Canvas canvas)
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

        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, new PngEncoder());
        return memoryStream.ToArray();
    }

    private static byte ToByte(float value)
    {
        return (byte)Math.Clamp(value * 255f, 0, 255);
    }
}