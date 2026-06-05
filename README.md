# Clc.Polaris.Api

`Clc.Polaris.Api` is a .NET helper package for calling the Polaris Integrated Library System (ILS) PAPI REST API from C# applications.

## Install

```bash
dotnet add package Clc.Polaris.Api --version 4.0.0-alpha.1
```

## Configure `PapiClient`

```csharp
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;

var settings = new PapiSettings
{
    Hostname = "https://polaris.example.org",
    AccessId = "your-access-id",
    AccessKey = "your-access-key",
    OrganizationId = 100,
    UserId = 1,
    WorkstationId = 1,
    PolarisOverrideAccount = new PolarisUser
    {
        Domain = "LIBRARY",
        Username = "staff.user",
        Password = "staff-password"
    }
};

var client = new PapiClient(settings);
```

`PapiSettings.PolarisOverrideAccount` is copied to `PapiClient.StaffOverrideAccount`. When `StaffOverrideAccount` is configured, protected requests and supported staff-override requests can acquire protected tokens automatically.

## Basic async usage

```csharp
using Clc.Polaris.Api.Models;

using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

var response = await client.BibKeywordSearchAsync(
    "octavia butler",
    branchId: settings.OrganizationId,
    page: 1,
    pageSize: 10,
    cancellationToken: cancellationTokenSource.Token);

if (response.Response.IsSuccessStatusCode)
{
    var results = response.Data;
    // Use the returned bibliographic search results.
}
```

Public client methods are async-only in 4.0 and should be awaited; do not block on them or look for synchronous alternatives. Pass a `CancellationToken` where appropriate so callers can cancel both the PAPI request and any required authentication request.

Protected-token-in-path endpoints use `ProtectedToken.Placeholder` internally. Most callers should use the provided client methods rather than constructing those paths manually.

## Local tests

Run non-integration tests from the repository root:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory!=Integration"
```

Integration tests require live Polaris dev credentials and local test data in `src/polaris-api-csharpTests/appsettings.Test.json`, so they are not run by default in CI. Keep this file local only: do not commit real secrets, and remember that `appsettings.Test.json` is ignored by git.

Run basic read-only integration tests only when you have local Polaris dev settings configured. This excludes protected read-only tests that require staff override credentials:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration&TestCategory!=MutatingIntegration&TestCategory!=ProtectedIntegration"
```

Run protected read-only integration tests only when your local settings include staff override credentials in `PapiSettings.PolarisOverrideAccount`:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=ProtectedIntegration&TestCategory!=MutatingIntegration"
```

Run mutating/destructive integration tests only against a disposable Polaris dev environment that is refreshed nightly or otherwise safe to dirty:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=MutatingIntegration"
```

`MutatingIntegration` tests may create or modify patron blocks, title lists, account entries, record-set entries, notes, or similar artifacts in Polaris. These artifacts may be left behind; cleanup is not guaranteed. Same-day collision avoidance is handled by unique test names and notes, not by assuming prior artifacts were removed. Protected or destructive tests require staff override credentials in `PapiSettings.PolarisOverrideAccount`.

A local `appsettings.Test.json` follows this shape:

```json
{
  "PapiSettings": {
    "AccessId": "your-access-id",
    "AccessKey": "your-access-key",
    "Hostname": "https://polaris.example.org",
    "PolarisOverrideAccount": {
      "Domain": "LIBRARY",
      "Username": "staff.user",
      "Password": "staff-password"
    }
  },
  "PatronId": 123456,
  "PatronBarcode": "12345678901234",
  "PatronPin": "1234",
  "FreeTextBlock": "Local integration test block",
  "PatronListName": "Local integration test list",
  "OrgEmail": "library@example.org"
}
```

## Migration guide

See [MIGRATION.md](MIGRATION.md) for 3.x-to-4.0 async migration notes.
