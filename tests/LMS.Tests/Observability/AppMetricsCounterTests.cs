using LMS.Common.Observability.Metrics;
using Xunit;

namespace LMS.Tests.Observability;

public class AppMetricsCounterTests
{
    [Fact]
    public void HttpRequestsCounter_ShouldEqualN_AfterNIncrements()
    {
        var metrics = new AppMetrics();

        for (int i = 0; i < 5; i++)
        {
            metrics.IncrementHttpRequests();
        }

        Assert.Equal(5, metrics.HttpRequestsTotal);
    }
}