using Microsoft.Playwright;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests.Smoke.UI;

[TestFixture]
[Category("UI")]
public sealed class LocalFixtureUiTests : UiTestBase
{
    [Test]
    public async Task HomePageDisplaysFixtureStatus()
    {
        await Page.GotoAsync(LocalFixtureServer.BaseUrl);

        Assert.That(await Page.TitleAsync(), Is.EqualTo("Automation Fixture"));
        Assert.That(await Page.Locator("#status").InnerTextAsync(), Is.EqualTo("Ready"));
    }
}