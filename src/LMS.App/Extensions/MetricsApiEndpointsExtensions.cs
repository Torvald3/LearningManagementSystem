using LMS.Common.Observability.Metrics;

namespace LMS.App.Extensions;

public static class MetricsEndpointsExtensions
{
    public static IEndpointRouteBuilder MapMetricsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/metrics", (AppMetrics metrics) => Results.Ok(new
            {
                httpRequestsTotal = metrics.HttpRequestsTotal,
                httpRequestsFailedTotal = metrics.HttpRequestsFailedTotal,
                activeUserSessions = metrics.ActiveUserSessions,
                coursesTotal = metrics.CoursesTotal,
                requestDurationsMs = metrics.RequestDurationsMs
            }))
            .WithTags("Observability");

        return app;
    }
}