---
name: plan-playwrightwebautomationframework
description: plan-playwrightWebAutomationFramework
disable-model-invocation: true
---
## Plan: Playwright Web Test Framework

Create a reusable C#/.NET 10 LTS test framework for web UI and HTTP API behavior using Playwright and NUnit. Generate ExtentReports HTML output, capture structured Serilog logs and failure artifacts, and keep target URLs and credentials configurable. The workspace is empty, so this is a starter framework; real application selectors, endpoints, auth flows, and acceptance tests will be added once an application contract is available.

**Steps**
1. Scaffold a solution and one NUnit test project, organized into `UI`, `API`, and shared support namespaces/folders. Use categories to allow UI and API slices to run independently while sharing configuration and reporting.
2. Add runtime configuration via `appsettings.json` plus environment-variable overrides (for example, `E2E_*`). Include app/API base URLs, selected browser, headless mode, timeouts, and artifact/report paths. Keep secrets out of checked-in files; validate required settings at startup.
3. Implement browser and API lifecycle support. Create isolated browser contexts/pages per UI test and Playwright `APIRequestContext` instances for standalone endpoint tests and UI setup helpers. Support Chromium and Firefox, with browser selection per run and isolated artifact/report names to avoid collisions.
4. Add NUnit lifecycle integration for ExtentReports HTML output and Serilog logging. Record test result, duration, exception, and sanitized diagnostic details; attach screenshots and Playwright traces for failed UI tests. Do not include credentials or sensitive payloads in logs/reports.
5. Add representative framework smoke tests that validate navigation/page assertions and API request/response assertions against configurable target URLs. Use explicit placeholders or a harmless health-check contract; do not invent app-specific business behavior.
6. Document prerequisites, browser installation, configuration/secrets, browser selection, test categories, `dotnet test` commands, artifact locations, and how to extend the templates. Keep CI provider workflows out of scope for this first pass.

**Relevant files**
- `c:\WS_23Oct\WebApp.Automation.sln` — solution entry point.
- `c:\WS_23Oct\tests\WebApp.Automation.Tests\WebApp.Automation.Tests.csproj` — NUnit, Playwright, ExtentReports, Serilog, and configuration dependencies.
- `c:\WS_23Oct\tests\WebApp.Automation.Tests\Configuration\` — settings model, loader, and validation.
- `c:\WS_23Oct\tests\WebApp.Automation.Tests\Infrastructure\` — browser/API context creation, NUnit hooks, reporting, logging, and artifact capture.
- `c:\WS_23Oct\tests\WebApp.Automation.Tests\UI\` and `...\API\` — categorized example test fixtures and extension points.
- `c:\WS_23Oct\tests\WebApp.Automation.Tests\appsettings.json` — non-secret defaults/placeholders only.
- `c:\WS_23Oct\README.md` — setup and run instructions.

**Verification**
1. Build the solution with `dotnet build` using the .NET 10 SDK.
2. Install Chromium and Firefox through the Playwright .NET browser-install script; run a small UI smoke test in both browser selections.
3. Run the API smoke test with a configured base URL and verify a failure is clear when required configuration is absent or invalid.
4. Run passing, failing, and skipped tests; confirm ExtentReports produces a readable HTML report with correct statuses, logs are emitted, and failed UI tests attach screenshots/traces without leaking secrets.
5. Verify NUnit category filtering and the documented local commands.

**Decisions**
- Reporting: ExtentReports, producing HTML directly from the NUnit run.
- Test organization: one project with distinct UI/API categories and shared helpers; include standalone API tests as well as API helpers for UI tests.
- Browsers: Chromium and Firefox.
- Configuration: checked-in non-secret defaults with environment-variable overrides; credentials only via environment/secrets.
- Logging: Serilog with structured console/file output and per-test context.
- Runtime: .NET 10 LTS.
- Execution: local CLI/IDE first; no provider-specific CI workflow in the initial scope.
- Scope boundary: framework and generic examples only. Real selectors, auth details, endpoint contracts, test data, and app-specific assertions require target application details.
