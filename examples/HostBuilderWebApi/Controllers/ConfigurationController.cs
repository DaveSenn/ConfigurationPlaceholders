using Microsoft.AspNetCore.Mvc;

namespace HostBuilderWebApi.Controllers;

[ApiController]
[Route( "[controller]" )]
public class ConfigurationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ConfigurationController( IConfiguration configuration ) =>
        _configuration = configuration;

    [HttpGet]
    public IActionResult Get() =>
        Ok( _configuration["HostInfo"] );
}