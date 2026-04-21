using LMS.App.Observability;
using LMS.Common.Observability.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LMS.Common.Observability.UnitTests.Observability;

public class RequestMetricsMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldIncrementHttpRequests_ForEveryRequest()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = context =>
        {
            nextCalled = true;
            context.Response.StatusCode = 200;
            return Task.CompletedTask;
        };

        var loggerMock = new Mock<ILogger<RequestMetricsMiddleware>>();
        var middleware = new RequestMetricsMiddleware(next, loggerMock.Object);
        var context = new DefaultHttpContext();
        var metrics = new AppMetrics();

        // Act
        await middleware.InvokeAsync(context, metrics);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(1, metrics.HttpRequestsTotal);
        Assert.Equal(0, metrics.HttpRequestsFailedTotal);
        Assert.Single(metrics.RequestDurationsMs);
    }

    [Fact]
    public async Task InvokeAsync_ShouldIncrementFailedRequests_WhenStatusCodeIs500()
    {
        // Arrange
        RequestDelegate next = context =>
        {
            context.Response.StatusCode = 500;
            return Task.CompletedTask;
        };

        var loggerMock = new Mock<ILogger<RequestMetricsMiddleware>>();
        var middleware = new RequestMetricsMiddleware(next, loggerMock.Object);
        var context = new DefaultHttpContext();
        var metrics = new AppMetrics();

        // Act
        await middleware.InvokeAsync(context, metrics);

        // Assert
        Assert.Equal(1, metrics.HttpRequestsTotal);
        Assert.Equal(1, metrics.HttpRequestsFailedTotal);
        Assert.Single(metrics.RequestDurationsMs);
    }

    [Fact]
    public async Task InvokeAsync_ShouldIncrementFailedRequests_WhenExceptionIsThrown()
    {
        // Arrange
        RequestDelegate next = _ => throw new InvalidOperationException("boom");

        var loggerMock = new Mock<ILogger<RequestMetricsMiddleware>>();
        var middleware = new RequestMetricsMiddleware(next, loggerMock.Object);
        var context = new DefaultHttpContext();
        var metrics = new AppMetrics();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            middleware.InvokeAsync(context, metrics));

        // Assert
        Assert.Equal(1, metrics.HttpRequestsTotal);
        Assert.Equal(1, metrics.HttpRequestsFailedTotal);
        Assert.Single(metrics.RequestDurationsMs);
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        RequestDelegate next = _ => throw new InvalidOperationException("boom");

        var loggerMock = new Mock<ILogger<RequestMetricsMiddleware>>();
        var middleware = new RequestMetricsMiddleware(next, loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Method = "POST";
        context.Request.Path = "/api/courses";
        var metrics = new AppMetrics();

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            middleware.InvokeAsync(context, metrics));

        // Assert
        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString()!.Contains("Unhandled request error")),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldRecordRequestDuration()
    {
        // Arrange
        RequestDelegate next = async context =>
        {
            await Task.Delay(10);
            context.Response.StatusCode = 200;
        };

        var loggerMock = new Mock<ILogger<RequestMetricsMiddleware>>();
        var middleware = new RequestMetricsMiddleware(next, loggerMock.Object);
        var context = new DefaultHttpContext();
        var metrics = new AppMetrics();

        // Act
        await middleware.InvokeAsync(context, metrics);

        // Assert
        Assert.Single(metrics.RequestDurationsMs);
        Assert.True(metrics.RequestDurationsMs.First() >= 0);
    }
}