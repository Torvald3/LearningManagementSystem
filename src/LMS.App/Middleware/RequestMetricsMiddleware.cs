using System.Diagnostics;
using LMS.Common.Observability.Metrics;

namespace LMS.App.Observability;

public class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestMetricsMiddleware> _logger;

    public RequestMetricsMiddleware(
        RequestDelegate next,
        ILogger<RequestMetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, AppMetrics metrics)
    {
        var stopwatch = Stopwatch.StartNew();
        metrics.IncrementHttpRequests();

        try
        {
            await _next(context);

            if (context.Response.StatusCode >= 500)
            {
                metrics.IncrementFailedRequests();
            }
        }
        catch (Exception ex)
        {
            metrics.IncrementFailedRequests();

            _logger.LogError(ex,
                "Unhandled request error. event={Event} method={Method} path={Path}",
                "http.request.unhandled_error",
                context.Request.Method,
                context.Request.Path);

            throw;
        }
        finally
        {
            stopwatch.Stop();
            metrics.ObserveRequestDuration(stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}