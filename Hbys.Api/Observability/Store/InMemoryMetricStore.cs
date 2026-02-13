using Hbys.Api.Observability.Models;
using System.Collections.Concurrent;

namespace Hbys.Api.Observability.Store;

public sealed class InMemoryMetricStore
{
    private readonly ConcurrentQueue<RequestMetric> _requests = new();
    private readonly ConcurrentQueue<DependencyMetric> _deps = new();

    public void Add(RequestMetric m) => _requests.Enqueue(m);
    public void Add(DependencyMetric m) => _deps.Enqueue(m);

    public IReadOnlyCollection<RequestMetric> GetRequests(int take = 200)
        => _requests.Reverse().Take(take).ToArray();

    public IReadOnlyCollection<DependencyMetric> GetDependencies(int take = 200)
        => _deps.Reverse().Take(take).ToArray();
}
