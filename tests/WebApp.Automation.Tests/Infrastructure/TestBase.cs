using Serilog;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests;

public abstract class TestBase
{
    protected AventStack.ExtentReports.ExtentTest ReportTest { get; private set; } = null!;
    protected ILogger TestLogger { get; private set; } = Log.Logger;

    [SetUp]
    public void BeginTest()
    {
        var context = TestContext.CurrentContext;
        ReportTest = AutomationRuntime.CreateTest(context.Test.Name);
        TestLogger = Log.ForContext("TestId", context.Test.ID)
            .ForContext("TestName", context.Test.Name);
        TestLogger.Information("Starting test");
    }

    [TearDown]
    public void CompleteTest()
    {
        var result = TestContext.CurrentContext.Result;
        var diagnostic = DiagnosticSanitizer.Sanitize(result.Message);

        if (result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            TestLogger.Error("Test failed: {Diagnostic}", diagnostic);
        }
        else
        {
            TestLogger.Information("Test completed with status {Status}", result.Outcome.Status);
        }

        AutomationRuntime.CompleteTest(ReportTest, result.Outcome.Status, diagnostic);
    }
}