using Hbys.Api.Observability.Http;
using Hbys.Api.Observability.Middleware;
using Hbys.Api.Observability.Store;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

// Swagger (UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Observability DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<InMemoryMetricStore>();
builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddTransient<RequestTimingMiddleware>();
builder.Services.AddTransient<DependencyTrackingHandler>();

// Dependency Configuration
var deps = builder.Configuration.GetSection("Dependencies");

// HttpClients + dependency tracking
builder.Services.AddHttpClient("LAB", c =>
{
    c.BaseAddress = new Uri(deps["LAB"]!);
})
.AddHttpMessageHandler<DependencyTrackingHandler>();

builder.Services.AddHttpClient("PACS", c =>
{
    c.BaseAddress = new Uri(deps["PACS"]!);
    c.Timeout = TimeSpan.FromSeconds(2); // PACS timeout demo için
})
.AddHttpMessageHandler<DependencyTrackingHandler>();

builder.Services.AddHttpClient("SMS", c =>
{
    c.BaseAddress = new Uri(deps["SMS"]!);
})
.AddHttpMessageHandler<DependencyTrackingHandler>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Observability middleware (ORDER MATTERS)
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();