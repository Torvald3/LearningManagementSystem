namespace LMS.Common.Observability.Logging;
public static class PiiMaskingHelper
{
    public static string MaskEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return "***";

        var parts = email.Split('@');
        if (parts.Length != 2)
            return "***";

        var local = parts[0];
        var domain = parts[1];

        if (local.Length <= 2)
            return $"**@{domain}";

        return $"{local[..2]}***@{domain}";
    }
}