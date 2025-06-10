using Api.DTOs;
using Core;
using Core.Drawing;
using Core.Drawing.Exporter;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MapGenController : Controller
{
    
    private readonly PngStreamCanvasExporter _exporter = new PngStreamCanvasExporter();

    [HttpPost]
    public IActionResult GenerateMap([FromBody] MapGenInput input)
    {
        var canvas = MapGenerator.GenerateMap(input.CommandsJson);
        var bytes = _exporter.Export(canvas);
        return File(bytes, "image/png", "map.png");
    }
}