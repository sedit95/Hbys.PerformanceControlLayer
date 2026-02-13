namespace Hbys.Api.Observability.Models;

public sealed record RequestMetric(
    DateTime OccurredAtUtc,
    string Path,
    string Method,
    int StatusCode,
    long DurationMs,
    string CorrelationId
);
