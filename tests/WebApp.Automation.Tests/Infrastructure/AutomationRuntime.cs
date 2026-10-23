using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Serilog;
using WebApp.Automation.Tests.Configuration;

namespace WebApp.Automation.Tests.Infrastructure;

internal static class AutomationRuntime
{
    private static readonly object ReportLock = new();
    private static ExtentReports? _extent;

    public static AutomationSettings Settings { get; private set; } = null!;

    public static void Initialize(AutomationSettings settings)
    {
        Settings = settings;

        var runId = $"{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}";
        var configuredReportPath = settings.ReportPath.Replace("{RunId}", runId, StringComparison.Ordinal);
        var reportPath = settings.ResolvePath(configuredReportPath);
        var logPath = settings.ResolvePath(settings.LogPath);
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        Directory.CreateDirectory(settings.ResolvePath(settings.ArtifactDirectory));

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, shared: true)
            .CreateLogger();

        var reporter = new ExtentSparkReporter(reportPath);
        _extent = new ExtentReports();
        _extent.AttachReporter(reporter);
    }

    public static ExtentTest CreateTest(string name)
    {
        lock (ReportLock)
        {
            return (_extent ?? throw new InvalidOperationException("The report runtime has not been initialized."))
                .CreateTest(name);
        }
    }

    public static void CompleteTest(ExtentTest test, NUnit.Framework.Interfaces.TestStatus status, string diagnostic)
    {
        lock (ReportLock)
        {
            switch (status)
            {
                case NUnit.Framework.Interfaces.TestStatus.Passed:
                    test.Pass("Test passed.");
                    break;
                case NUnit.Framework.Interfaces.TestStatus.Skipped:
                case NUnit.Framework.Interfaces.TestStatus.Inconclusive:
                    test.Skip(diagnostic);
                    break;
                default:
                    test.Fail(diagnostic);
                    break;
            }
        }
    }

    public static void Flush()
    {
        lock (ReportLock)
        {
            _extent?.Flush();
            _extent = null;
        }

        Log.CloseAndFlush();
    }
}