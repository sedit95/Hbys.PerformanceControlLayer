using Hbys.Api.Observability.Models;
using Hbys.Api.Observability.Store;

namespace Hbys.Api.Observability.Middleware;

public sealed class RequestTimingMiddleware : IMiddleware
{
    private readonly InMemoryMetricStore _store;

    public RequestTimingMiddleware(InMemoryMetricStore store) => _store = store;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        await next(context);

        sw.Stop();

        var cid = (string?)context.Items[CorrelationIdMiddleware.HeaderName] ?? "no-cid";

        _store.Add(new RequestMetric(
            OccurredAtUtc: DateTime.UtcNow,
            Path: context.Request.Path.Value ?? "",
            Method: context.Request.Method,
            StatusCode: context.Response.StatusCode,
            DurationMs: sw.ElapsedMilliseconds,
            CorrelationId: cid
        ));
    }
}
