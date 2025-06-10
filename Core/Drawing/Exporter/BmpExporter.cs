using System.Numerics;

namespace Core.Drawing.Exporter;

public class BmpExporter : ICanvasExporter
{
    public void Export(string filePath, Canvas canvas)
    {
        int width = canvas.Width;
        int height = canvas.Height;
        int rowPadding = (4 - (width * 3) % 4) % 4;
        int rowSize = width * 3 + rowPadding;
        int pixelDataSize = rowSize * height;
        int fileSize = 54 + pixelDataSize;

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        using var writer = new BinaryWriter(stream);

        // BITMAP FILE HEADER
        writer.Write((ushort)0x4D42); // Signature 'BM'
        writer.Write(fileSize); // File size
        writer.Write(0); // Reserved
        writer.Write(54); // Pixel data offset

        // DIB HEADER (BITMAPINFOHEADER)
        writer.Write(40); // Header size
        writer.Write(width); // Width
        writer.Write(height); // Height
        writer.Write((ushort)1); // Color planes
        writer.Write((ushort)24); // Bits per pixel
        writer.Write(0); // Compression (none)
        writer.Write(pixelDataSize); // Image size
        writer.Write(2835); // Horizontal resolution (72 DPI)
        writer.Write(2835); // Vertical resolution (72 DPI)
        writer.Write(0); // Colors in palette
        writer.Write(0); // Important colors

        // PIXEL DATA (Bottom-up)
        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 color = canvas.GetPixelUnsafe(x, y);

                byte r = (byte)(Math.Clamp(color.X, 0, 1) * 255);
                byte g = (byte)(Math.Clamp(color.Y, 0, 1) * 255);
                byte b = (byte)(Math.Clamp(color.Z, 0, 1) * 255);

                writer.Write(b);
                writer.Write(g);
                writer.Write(r);
            }

            // Padding
            for (int i = 0; i < rowPadding; i++)
                writer.Write((byte)0);
        }
    }
}