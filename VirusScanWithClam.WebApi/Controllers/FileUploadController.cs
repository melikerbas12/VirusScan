using Microsoft.AspNetCore.Mvc;
using VirusScanWithClam.WebApi.Filter;

namespace VirusScanWithClam.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public FileUploadController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [ProducesResponseType(200, Type = typeof(int))]
    [HttpPost]
    [CheckFile]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        return Ok();
    }
}
