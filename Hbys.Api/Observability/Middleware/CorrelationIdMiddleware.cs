namespace Hbys.Api.Observability.Middleware;

public sealed class CorrelationIdMiddleware : IMiddleware
{
    public const string HeaderName = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var cid = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(cid))
            cid = Guid.NewGuid().ToString("N"); // 32 char, kısa ve temiz

        // request boyunca erişmek için
        context.Items[HeaderName] = cid;

        // client da görsün diye response header
        context.Response.Headers[HeaderName] = cid;

        await next(context);
    }
}
