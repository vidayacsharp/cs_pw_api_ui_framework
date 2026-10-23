using System.Text.RegularExpressions;

namespace WebApp.Automation.Tests.Infrastructure;

internal static partial class DiagnosticSanitizer
{
    public static string Sanitize(string? diagnostic)
    {
        if (string.IsNullOrWhiteSpace(diagnostic))
        {
            return "No diagnostic details were provided.";
        }

        return BearerToken().Replace(SensitiveValues().Replace(diagnostic, "$1$2[REDACTED]"), "$1[REDACTED]");
    }

    [GeneratedRegex("(?i)(password|token|secret|authorization|api[_-]?key)(\\s*[=:]\\s*)([^&\\s,;]+)")]
    private static partial Regex SensitiveValues();

    [GeneratedRegex("(?i)(Bearer\\s+)[A-Za-z0-9._~+/=-]+")]
    private static partial Regex BearerToken();
}