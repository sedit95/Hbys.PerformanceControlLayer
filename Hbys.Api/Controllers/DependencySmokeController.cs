using Microsoft.AspNetCore.Mvc;

namespace Hbys.Api.Controllers;

[ApiController]
[Route("api/smoke")]
public sealed class DependencySmokeController : ControllerBase
{
    private static readonly HttpRequestOptionsKey<string> TargetSystemKey = new("TargetSystem");
    private readonly IHttpClientFactory _factory;

    public DependencySmokeController(IHttpClientFactory factory) => _factory = factory;

    [HttpPost("lab")]
    public async Task<IActionResult> Lab(CancellationToken ct)
    {
        var client = _factory.CreateClient("LAB");

        using var req = new HttpRequestMessage(HttpMethod.Post, "lab/request");
        req.Options.Set(TargetSystemKey, "LAB");

        var resp = await client.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        return StatusCode((int)resp.StatusCode, body);
    }

    [HttpGet("pacs/{id}")]
    public async Task<IActionResult> Pacs(string id, CancellationToken ct)
    {
        var client = _factory.CreateClient("PACS");

        using var req = new HttpRequestMessage(HttpMethod.Get, $"pacs/image/{Uri.EscapeDataString(id)}");
        req.Options.Set(TargetSystemKey, "PACS");

        var resp = await client.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        return StatusCode((int)resp.StatusCode, body);
    }

    [HttpPost("sms")]
    public async Task<IActionResult> Sms(CancellationToken ct)
    {
        var client = _factory.CreateClient("SMS");

        using var req = new HttpRequestMessage(HttpMethod.Post, "sms/send");
        req.Options.Set(TargetSystemKey, "SMS");

        var resp = await client.SendAsync(req, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        return StatusCode((int)resp.StatusCode, body);
    }
}
