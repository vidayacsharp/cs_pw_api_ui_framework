using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WebApp.Automation.Tests.Infrastructure;

internal static class LocalFixtureServer
{
    private static WebApplication? _application;

    public static string BaseUrl { get; private set; } = string.Empty;

    public static async Task StartAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory
        });
        builder.Logging.ClearProviders();
        builder.WebHost.ConfigureKestrel(options => options.Listen(IPAddress.Loopback, 0));

        var application = builder.Build();
        application.MapGet("/", () => Results.Content(
            "<!doctype html><html><head><title>Automation Fixture</title></head>" +
            "<body><main><h1>Automation Fixture</h1><p id=\"status\">Ready</p></main></body></html>",
            "text/html"));
        application.MapGet("/api/health", () => Results.Json(new { status = "ok", service = "automation-fixture" }));

        await application.StartAsync();
        var address = application.Services.GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()?.Addresses.SingleOrDefault();

        if (string.IsNullOrWhiteSpace(address))
        {
            await application.DisposeAsync();
            throw new InvalidOperationException("The local test fixture did not publish its listening address.");
        }

        _application = application;
        BaseUrl = address.TrimEnd('/');
    }

    public static async Task StopAsync()
    {
        if (_application is not null)
        {
            await _application.StopAsync();
            await _application.DisposeAsync();
            _application = null;
            BaseUrl = string.Empty;
        }
    }
}