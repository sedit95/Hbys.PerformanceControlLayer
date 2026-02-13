using Microsoft.AspNetCore.Mvc;

namespace Hbys.Api.Controllers;

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { ok = true, service = "HBYS", at = DateTime.UtcNow });
}
