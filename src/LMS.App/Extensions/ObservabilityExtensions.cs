using LMS.App.Observability;
using LMS.Common.Observability.Metrics;

namespace LMS.App.Extensions;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddSingleton<AppMetrics>();
        return services;
    }

    public static IApplicationBuilder UseRequestMetrics(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestMetricsMiddleware>();
        return app;
    }
}