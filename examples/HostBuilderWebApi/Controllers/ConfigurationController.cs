using Microsoft.AspNetCore.Mvc;

namespace HostBuilderWebApi.Controllers;

[ApiController]
[Route( "[controller]" )]
public class ConfigurationController( IConfiguration configuration ) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok( configuration["HostInfo"] );
}