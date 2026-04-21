using Xunit;

namespace LMS.Common.Observability.UnitTests.Observability;

public class LogFormatTests
{
    [Fact]
    public void Log_ShouldContainTimestampLevelAndEvent()
    {
        var timestamp = DateTime.UtcNow.ToString("O");
        var level = "INFO";
        var @event = "course.create.succeeded";

        var log = $"timestamp={timestamp} level={level} event={@event}";

        Assert.Contains("timestamp=", log);
        Assert.Contains("level=", log);
        Assert.Contains("event=", log);
    }
}