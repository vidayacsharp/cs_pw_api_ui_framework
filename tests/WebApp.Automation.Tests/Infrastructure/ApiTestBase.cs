using Microsoft.Playwright;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests;

public abstract class ApiTestBase : TestBase
{
    private IPlaywright? _playwright;
    private IAPIRequestContext? _api;

    protected IAPIRequestContext Api => _api ?? throw new InvalidOperationException("The API context has not been initialized.");

    protected virtual string ApiBaseUrl => AutomationRuntime.Settings.GetApiBaseUrl();

    [SetUp]
    public async Task CreateApiContextAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _api = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = ApiBaseUrl
        });
    }

    [TearDown]
    public async Task DisposeApiContextAsync()
    {
        if (_api is not null)
        {
            await _api.DisposeAsync();
        }

        _playwright?.Dispose();
    }
}