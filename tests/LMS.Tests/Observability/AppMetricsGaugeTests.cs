using LMS.Common.Observability.Metrics;
using Xunit;

namespace LMS.Tests.Observability;

public class AppMetricsGaugeTests
{
    [Fact]
    public void ActiveSessionsGauge_ShouldIncreaseAndDecrease()
    {
        var metrics = new AppMetrics();

        metrics.SessionOpened();
        Assert.Equal(1, metrics.ActiveUserSessions);

        metrics.SessionClosed();
        Assert.Equal(0, metrics.ActiveUserSessions);
    }
}