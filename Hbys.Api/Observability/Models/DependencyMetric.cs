namespace Hbys.Api.Observability.Models;

public sealed record DependencyMetric(
    DateTime OccurredAtUtc,
    string TargetSystem,     // LAB / PACS / SMS
    string TargetPath,       // /lab/request ...
    string Method,
    int? StatusCode,         // timeout olursa null
    long DurationMs,
    bool IsSuccess,
    bool IsTimeout,
    string CorrelationId
);
