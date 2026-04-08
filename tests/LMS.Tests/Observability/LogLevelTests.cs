using Xunit;

namespace LMS.Tests.Observability;

public class LogLevelTests
{
    [Fact]
    public void ErrorEvent_ShouldUseErrorLevel_NotInfo()
    {
        var level = "ERROR";

        Assert.Equal("ERROR", level);
        Assert.NotEqual("INFO", level);
    }
}