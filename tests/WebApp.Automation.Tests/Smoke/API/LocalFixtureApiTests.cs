using System.Text.Json;
using WebApp.Automation.Tests.Infrastructure;

namespace WebApp.Automation.Tests.Smoke.API;

[TestFixture]
[Category("API")]
public sealed class LocalFixtureApiTests : ApiTestBase
{
    protected override string ApiBaseUrl => LocalFixtureServer.BaseUrl;

    [Test]
    public async Task HealthEndpointReturnsOk()
    {
        var response = await Api.GetAsync("/api/health");

        Assert.That(response.Status, Is.EqualTo(200));
        using var payload = JsonDocument.Parse(await response.TextAsync());
        Assert.That(payload.RootElement.GetProperty("status").GetString(), Is.EqualTo("ok"));
    }
}