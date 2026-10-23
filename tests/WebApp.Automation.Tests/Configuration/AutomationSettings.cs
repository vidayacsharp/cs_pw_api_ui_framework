using Microsoft.Extensions.Configuration;

namespace WebApp.Automation.Tests.Configuration;

public sealed class AutomationSettings
{
    public string Browser { get; init; } = "chromium";
    public bool Headless { get; init; } = true;
    public int TimeoutMs { get; init; } = 30000;
    public string ArtifactDirectory { get; init; } = "artifacts";
    public string ReportPath { get; init; } = "artifacts/extent-report-{RunId}.html";
    public string LogPath { get; init; } = "artifacts/logs/automation-.log";
    public string? BaseUrl { get; init; }
    public string? ApiBaseUrl { get; init; }

    public static AutomationSettings Load()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables("E2E_")
            .Build();

        var settings = configuration.GetSection("Automation").Get<AutomationSettings>() ?? new();
        settings.Validate();
        return settings;
    }

    public string GetApiBaseUrl()
    {
        var url = string.IsNullOrWhiteSpace(ApiBaseUrl) ? BaseUrl : ApiBaseUrl;
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException(
                "Configure Automation:ApiBaseUrl or Automation:BaseUrl (environment prefix: E2E_) before running app API tests.");
        }

        return url;
    }

    public string ResolvePath(string path)
    {
        return Path.IsPathRooted(path)
            ? path
            : Path.GetFullPath(Path.Combine(FindWorkspaceRoot(), path));
    }

    private static string FindWorkspaceRoot()
    {
        foreach (var startingPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(startingPath);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "WebApp.Automation.slnx")) ||
                    File.Exists(Path.Combine(directory.FullName, "WebApp.Automation.sln")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        return Directory.GetCurrentDirectory();
    }

    private void Validate()
    {
        if (Browser is not ("chromium" or "firefox"))
        {
            throw new InvalidOperationException("Automation:Browser must be either 'chromium' or 'firefox'.");
        }

        if (TimeoutMs <= 0)
        {
            throw new InvalidOperationException("Automation:TimeoutMs must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(ArtifactDirectory) ||
            string.IsNullOrWhiteSpace(ReportPath) ||
            string.IsNullOrWhiteSpace(LogPath))
        {
            throw new InvalidOperationException(
                "Automation:ArtifactDirectory, Automation:ReportPath, and Automation:LogPath must not be empty.");
        }
    }
}