using Hbys.Api.Observability.Store;
using Microsoft.AspNetCore.Mvc;

namespace Hbys.Api.Controllers;

[ApiController]
[Route("metrics/debug")]
public sealed class MetricsDebugController : ControllerBase
{
    private readonly InMemoryMetricStore _store;
    public MetricsDebugController(InMemoryMetricStore store) => _store = store;

    [HttpGet("requests")]
    public IActionResult Requests([FromQuery] int take = 50)
        => Ok(_store.GetRequests(take));

    [HttpGet("deps")]
    public IActionResult Deps([FromQuery] int take = 50)
        => Ok(_store.GetDependencies(take));
}


