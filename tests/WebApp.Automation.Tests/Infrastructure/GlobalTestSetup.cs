using WebApp.Automation.Tests.Configuration;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests;

[SetUpFixture]
public sealed class GlobalTestSetup
{
    [OneTimeSetUp]
    public void Initialize()
    {
        AutomationRuntime.Initialize(AutomationSettings.Load());
    }

    [OneTimeTearDown]
    public void Shutdown()
    {
        AutomationRuntime.Flush();
    }
}