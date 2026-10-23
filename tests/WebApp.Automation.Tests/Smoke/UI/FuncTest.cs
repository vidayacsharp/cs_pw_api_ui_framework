using Microsoft.Playwright;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests.Smoke.UI;

[TestFixture]
[Category("UI")]
[Category("FS")]
public sealed class FuncTest : UiTestBase
{
    [Test]
    public async Task TC001_BbcHomePageLoadsSuccessfully()
    {
        var response = await OpenHomePageAsync();

        Assert.That(response.Ok, Is.True, $"BBC homepage returned HTTP {response.Status}.");
        Assert.That(
            await Page.Locator("#bbc-header")
                .GetByRole(AriaRole.Link, new() { Name = "British Broadcasting Corporation" })
                .IsVisibleAsync(),
            Is.True,
            "BBC logo link should be visible.");
        Assert.That(await Page.GetByRole(AriaRole.Button, new() { Name = "Open menu" }).IsVisibleAsync(), Is.True);
        Assert.That(await Page.Locator("main").IsVisibleAsync(), Is.True);
    }

    [Test]
    public async Task TC002_SearchReturnsTechnologyArticles()
    {
        await OpenHomePageAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open menu" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Site search" }).FillAsync("Technology");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Search", Exact = true }).ClickAsync();
        await Page.WaitForURLAsync("**/search**");

        var resultLinks = Page.Locator("main a[href*='/news/articles/']");
        await resultLinks.First.WaitForAsync();
        var resultTitles = await resultLinks.AllInnerTextsAsync();

        Assert.That(
            resultTitles.Any(title => title.Contains("Technology", StringComparison.OrdinalIgnoreCase)),
            Is.True,
            "Search results did not contain an article related to Technology.");
    }

    [Test]
    public async Task TC003_NewsSectionOpensFromNavigation()
    {
        await OpenHomePageAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Open menu" }).ClickAsync();
        await Page.Locator("[data-testid='level0NavButton-/news']").ClickAsync();
        await Page.GetByTestId("level0NavContent-/news")
            .GetByRole(AriaRole.Link, new() { Name = "News" })
            .ClickAsync();
        await Page.WaitForURLAsync("**/news**");

        Assert.That(Page.Url, Does.Contain("/news"));
        Assert.That(await Page.Locator("main h1, main h2").CountAsync(), Is.GreaterThan(0));
    }

    [Test]
    public async Task TC004_NewsArticleOpensWithTitleAndContent()
    {
        await OpenHomePageAsync();

        var articleLinks = Page.Locator("main a[href*='/news/articles/']");
        await articleLinks.First.WaitForAsync();
        await articleLinks.First.ClickAsync();

        var articleHeading = Page.Locator("main h1");
        await articleHeading.WaitForAsync();
        Assert.That(await articleHeading.InnerTextAsync(), Is.Not.Empty);
        Assert.That(Page.Url, Does.Contain("/news/articles/"));
    }

    [Test]
    public async Task TC005_MobileLayoutHasAccessibleNavigationWithoutHorizontalOverflow()
    {
        await Page.SetViewportSizeAsync(390, 844);
        await OpenHomePageAsync();

        var documentWidth = await Page.EvaluateAsync<int>("() => document.documentElement.scrollWidth");
        Assert.That(documentWidth, Is.LessThanOrEqualTo(390), "Homepage requires horizontal scrolling at mobile width.");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Open menu" }).ClickAsync();
        Assert.That(
            await Page.GetByRole(AriaRole.Textbox, new() { Name = "Site search" }).IsVisibleAsync(),
            Is.True,
            "Search/navigation controls should be accessible from the mobile menu.");
    }

    private async Task<IResponse> OpenHomePageAsync()
    {
        var configuredUrl = new Uri(AutomationRuntime.Settings.GetBaseUrl());
        var homeUrl = configuredUrl.GetLeftPart(UriPartial.Authority) + "/";
        var response = await Page.GotoAsync(homeUrl);

        Assert.That(response, Is.Not.Null, $"No HTTP response was received from {homeUrl}.");
        return response!;
    }
}
