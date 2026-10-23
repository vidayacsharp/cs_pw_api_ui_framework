using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests.Smoke;

[SetUpFixture]
public sealed class LocalFixtureSetup
{
    [OneTimeSetUp]
    public Task StartAsync()
    {
        return LocalFixtureServer.StartAsync();
    }

    [OneTimeTearDown]
    public Task StopAsync()
    {
        return LocalFixtureServer.StopAsync();
    }
}