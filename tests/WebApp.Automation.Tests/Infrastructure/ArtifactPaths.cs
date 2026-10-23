using System.Text.RegularExpressions;
using WebApp.Automation.Tests.Configuration;

namespace WebApp.Automation.Tests.Infrastructure;

internal static partial class ArtifactPaths
{
    public static string Create(AutomationSettings settings, string extension)
    {
        var directory = settings.ResolvePath(settings.ArtifactDirectory);
        Directory.CreateDirectory(directory);

        var testName = SafeName(TestContext.CurrentContext.Test.Name);
        return Path.Combine(directory, $"{testName}-{Guid.NewGuid():N}.{extension.TrimStart('.')}");
    }

    private static string SafeName(string name)
    {
        var safeName = InvalidFileNameCharacters().Replace(name, "-").Trim('-', '.');
        return string.IsNullOrWhiteSpace(safeName) ? "test" : safeName;
    }

    [GeneratedRegex("[^a-zA-Z0-9_-]+")]
    private static partial Regex InvalidFileNameCharacters();
}