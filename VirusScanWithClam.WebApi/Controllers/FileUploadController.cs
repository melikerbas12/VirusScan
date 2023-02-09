using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VirusScanWithClam.WebApi.Filter;
using VirusScanWithClam.WebApi.Models;

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
    [ServiceFilter(typeof(CheckFileAttribute))]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var a = "aaa";
        return Ok(a);
    }
}
