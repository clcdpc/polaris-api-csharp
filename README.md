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

Run integration tests only when you have local Polaris settings configured:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

Integration tests require local Polaris credentials and fixture data in `src/polaris-api-csharpTests/appsettings.Test.json` or equivalent environment variables, so they are not run by default in CI. Do not commit real secrets; `src/polaris-api-csharpTests/appsettings.Test.json` is ignored by git.

Copy `src/polaris-api-csharpTests/appsettings.Test.example.json` to `src/polaris-api-csharpTests/appsettings.Test.json` for local runs, or set .NET-style environment variables such as `PapiSettings__Hostname`, `PapiSettings__AccessId`, `PapiSettings__AccessKey`, `TestSettings__PatronBarcode`, and `IntegrationTestOptions__EnableMutatingIntegrationTests`. See `src/polaris-api-csharpTests/README.md` for the full integration-test configuration guide and coverage map.

## Migration guide

See [MIGRATION.md](MIGRATION.md) for 3.x-to-4.0 async migration notes.
