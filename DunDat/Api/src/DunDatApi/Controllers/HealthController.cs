using Microsoft.AspNetCore.Mvc;

namespace DunDatApi.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(200)]
    public IActionResult HealthCheck() => Ok();
}