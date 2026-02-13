using Hbys.Api.Observability.Middleware;
using Hbys.Api.Observability.Models;
using Hbys.Api.Observability.Store;

namespace Hbys.Api.Observability.Http;

public sealed class DependencyTrackingHandler : DelegatingHandler
{
    private readonly InMemoryMetricStore _store;
    private readonly IHttpContextAccessor _ctx;

    public DependencyTrackingHandler(InMemoryMetricStore store, IHttpContextAccessor ctx)
    {
        _store = store;
        _ctx = ctx;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        // CorrelationId (inbound request'ten taşır)
        var httpContext = _ctx.HttpContext;
        var cid = (string?)httpContext?.Items[CorrelationIdMiddleware.HeaderName] ?? Guid.NewGuid().ToString("N");

        // downstream'a propagate et
        if (!request.Headers.Contains(CorrelationIdMiddleware.HeaderName))
            request.Headers.Add(CorrelationIdMiddleware.HeaderName, cid);

        // TargetSystem'i HttpClient ismiyle belirleyeceğiz (B/2.6'da set edeceğiz)
        var targetSystem = request.Options.TryGetValue(new HttpRequestOptionsKey<string>("TargetSystem"), out var ts)
            ? ts
            : "UNKNOWN";

        try
        {
            var resp = await base.SendAsync(request, cancellationToken);
            sw.Stop();

            _store.Add(new DependencyMetric(
                OccurredAtUtc: DateTime.UtcNow,
                TargetSystem: targetSystem,
                TargetPath: request.RequestUri?.AbsolutePath ?? "",
                Method: request.Method.Method,
                StatusCode: (int)resp.StatusCode,
                DurationMs: sw.ElapsedMilliseconds,
                IsSuccess: resp.IsSuccessStatusCode,
                IsTimeout: false,
                CorrelationId: cid
            ));

            return resp;
        }
        catch (TaskCanceledException)
        {
            sw.Stop();

            _store.Add(new DependencyMetric(
                OccurredAtUtc: DateTime.UtcNow,
                TargetSystem: targetSystem,
                TargetPath: request.RequestUri?.AbsolutePath ?? "",
                Method: request.Method.Method,
                StatusCode: null,
                DurationMs: sw.ElapsedMilliseconds,
                IsSuccess: false,
                IsTimeout: true,
                CorrelationId: cid
            ));

            throw;
        }
        catch
        {
            sw.Stop();

            _store.Add(new DependencyMetric(
                OccurredAtUtc: DateTime.UtcNow,
                TargetSystem: targetSystem,
                TargetPath: request.RequestUri?.AbsolutePath ?? "",
                Method: request.Method.Method,
                StatusCode: null,
                DurationMs: sw.ElapsedMilliseconds,
                IsSuccess: false,
                IsTimeout: false,
                CorrelationId: cid
            ));

            throw;
        }
    }
}
