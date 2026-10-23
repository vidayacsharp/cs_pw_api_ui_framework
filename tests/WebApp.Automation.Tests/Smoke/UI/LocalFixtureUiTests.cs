using Microsoft.Playwright;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests.Smoke.UI;

[TestFixture]
[Category("UI")]
public sealed class LocalFixtureUiTests : UiTestBase
{
    [Test]    
    [Category("DefaultTest")]
    public async Task HomePageDisplaysFixtureStatus()
    {
        await Page.GotoAsync(LocalFixtureServer.BaseUrl);

        Assert.That(await Page.TitleAsync(), Is.EqualTo("Automation Fixture"));
        Assert.That(await Page.Locator("#status").InnerTextAsync(), Is.EqualTo("Ready"));
    }

    [Test]
    [Category("UnitTest")]
    public async Task ConfiguredBaseUrlLoads()
    {
        var response = await Page.GotoAsync(AutomationRuntime.Settings.GetBaseUrl());

        Assert.That(response, Is.Not.Null);
        Assert.That(
            response!.Ok,
            Is.True,
            $"Expected a successful response from {response.Url}, but received {(int)response.Status} {response.StatusText}.");
    }
}