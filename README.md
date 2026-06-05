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

Run read-only integration tests only when you have live Polaris dev credentials and local Polaris settings configured:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration&TestCategory!=DisposableIntegration"
```

Integration tests require live Polaris dev credentials and test data in `src/polaris-api-csharpTests/appsettings.Test.json`, so they are not run by default in CI. Keep `appsettings.Test.json` local only: do not commit real secrets or environment-specific test settings. The file is ignored by git.

Run disposable/destructive integration tests separately, and only against a disposable or nightly-refreshed Polaris dev environment:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=DisposableIntegration"
```

`DisposableIntegration` tests may create or modify patron blocks, title lists, account entries, record-set entries, notes, or similar Polaris artifacts. These tests are intended for disposable dev data only, artifacts may be left behind, and cleanup is not guaranteed. Same-day collision avoidance is handled by unique test names and notes rather than by assuming that prior artifacts were removed. Staff override credentials in `PapiSettings.PolarisOverrideAccount` are required for protected or destructive tests.

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
