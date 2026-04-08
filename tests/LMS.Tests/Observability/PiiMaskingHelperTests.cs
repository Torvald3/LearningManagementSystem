using LMS.Common.Observability;
using LMS.Common.Observability.Logging;
using Xunit;

namespace LMS.Tests.Observability;

public class PiiMaskingHelperTests
{
    [Fact]
    public void MaskEmail_ShouldHideFullEmail()
    {
        var email = "john.doe@gmail.com";

        var result = PiiMaskingHelper.MaskEmail(email);

        Assert.NotEqual(email, result);
        Assert.Contains("@gmail.com", result);
        Assert.DoesNotContain("john.doe@gmail.com", result);
    }
}