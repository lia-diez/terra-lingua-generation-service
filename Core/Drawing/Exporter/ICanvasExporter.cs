namespace Core.Drawing.Exporter;

public interface ICanvasExporter
{
    public void Export(string filePath, Canvas canvas);
}