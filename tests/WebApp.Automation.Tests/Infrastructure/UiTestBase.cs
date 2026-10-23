using Microsoft.Playwright;
using Serilog;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests;

public abstract class UiTestBase : TestBase
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _browserContext;
    private IPage? _page;

    protected IPage Page => _page ?? throw new InvalidOperationException("The browser page has not been initialized.");

    [SetUp]
    public async Task LaunchBrowserAsync()
    {
        _playwright = await Playwright.CreateAsync();
        var browserType = AutomationRuntime.Settings.Browser switch
        {
            "chromium" => _playwright.Chromium,
            "firefox" => _playwright.Firefox,
            _ => throw new InvalidOperationException("Unsupported browser configured.")
        };

        _browser = await browserType.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = AutomationRuntime.Settings.Headless
        });
        _browserContext = await _browser.NewContextAsync();
        await _browserContext.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
        _page = await _browserContext.NewPageAsync();
        _page.SetDefaultTimeout(AutomationRuntime.Settings.TimeoutMs);
    }

    [TearDown]
    public async Task CloseBrowserAsync()
    {
        var failed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed;
        var settings = AutomationRuntime.Settings;
        var capturePassedScreenshot =
            TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed &&
            settings.CaptureScreenshotsOnPass;

        if (_browserContext is not null)
        {
            if ((failed || capturePassedScreenshot) && _page is not null)
            {
                try
                {
                    var screenshotPath = ArtifactPaths.Create(settings, ".png");
                    await _page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
                    ReportTest.AddScreenCaptureFromPath(screenshotPath);
                    TestLogger.Information("Captured test screenshot at {ScreenshotPath}", screenshotPath);
                }
                catch (Exception exception)
                {
                    TestLogger.Warning(
                        "Could not capture {TestOutcome} screenshot ({ErrorType})",
                        failed ? "failure" : "passing",
                        exception.GetType().Name);
                }
            }

            try
            {
                if (failed)
                {
                    var tracePath = ArtifactPaths.Create(settings, ".zip");
                    await _browserContext.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
                    ReportTest.Info($"Playwright trace: {tracePath}");
                }
                else
                {
                    await _browserContext.Tracing.StopAsync();
                }
            }
            catch (Exception exception)
            {
                TestLogger.Warning("Could not finalize Playwright trace ({ErrorType})", exception.GetType().Name);
            }

            await _browserContext.CloseAsync();
        }

        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }
}