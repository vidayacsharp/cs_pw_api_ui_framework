# Web App Automation Framework

A .NET 10 test framework for browser UI and HTTP API checks using Playwright and NUnit. It includes Chromium and Firefox support, ExtentReports HTML output, Serilog console/file logs, and a self-hosted local fixture for offline smoke tests.

## Prerequisites

- .NET 10 SDK
- PowerShell (`pwsh`) for installing Playwright browsers

## Install browsers

Build once, then install the browser binaries:

```powershell
dotnet build WebApp.Automation.slnx
pwsh tests/WebApp.Automation.Tests/bin/Debug/net10.0/playwright.ps1 install chromium firefox
```

## Run tests

```powershell
dotnet test WebApp.Automation.slnx
dotnet test WebApp.Automation.slnx --filter "TestCategory=UI"
dotnet test WebApp.Automation.slnx --filter "TestCategory=API"
```

The local smoke tests start a Kestrel fixture on a dynamically assigned loopback port. They do not call public or production services.

Choose the browser for UI tests with an environment override:

```powershell -- for browser change
$env:E2E_Automation__Browser = "firefox"
dotnet test WebApp.Automation.slnx --filter "TestCategory=UI"
Remove-Item Env:E2E_Automation__Browser
```
```powershell -- for headless change
$env:E2E_Automation__Headless = "false"
dotnet test .\WebApp.Automation.slnx --filter "TestCategory=UI"
Remove-Item Env:E2E_Automation__Headless
```

Supported browser values are `chromium` and `firefox`. Set `E2E_Automation__Headless=false` to show the browser. Browser selection applies to the UI tests; the API tests do not launch a browser.

## Configuration

Non-secret defaults are in `tests/WebApp.Automation.Tests/appsettings.json`. Environment variables with the `E2E_` prefix override them. For example:

```powershell
$env:E2E_Automation__ApiBaseUrl = "https://test.example.invalid"
$env:E2E_Automation__BaseUrl = "https://test.example.invalid"
```

The sample tests use only the local fixture. `BaseUrl` and `ApiBaseUrl` are extension points for application-specific tests. Add credentials through your environment or secret store; never commit them or send them in chat. Keep generated logs, reports, screenshots, and traces access-controlled because they may contain application data.

Settings include browser, headless mode, optional screenshots for passed UI tests (`Automation:CaptureScreenshotsOnPass`), timeout, artifact directory, report path, log path, and optional application/API base URLs. Invalid browser and timeout values fail fast with a configuration error.

## Outputs

- ExtentReports HTML: `artifacts/extent-report-<UTC timestamp>-<run-id>.html` in the solution-root `artifacts/` directory (unique per run)
- Serilog rolling files: `artifacts/logs/`
- UI screenshots (failures always, passing tests when `CaptureScreenshotsOnPass` is enabled) and Playwright traces: `artifacts/`. Screenshots are attached to the ExtentReports test entry.

UI failures attach screenshots to the HTML report and save Playwright traces for inspection with `pwsh <path-to-playwright.ps1> show-trace <trace.zip>`. API test failures are recorded in the report and log. The default `Automation:ReportPath` is `artifacts/extent-report-{RunId}.html`; `{RunId}` is replaced with a UTC timestamp and unique identifier. Include `{RunId}` in custom paths to retain per-run report isolation; a custom path without the token is used as-is.

## Add application tests

Create UI fixtures deriving from `UiTestBase` and API fixtures deriving from `ApiTestBase`. Set `E2E_Automation__BaseUrl` and/or `E2E_Automation__ApiBaseUrl` in the test environment, add NUnit `[Category("UI")]` or `[Category("API")]`, and use the app's approved test environment and contracts. Do not log authorization headers, secrets, or sensitive response bodies.
