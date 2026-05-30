# Clc.Polaris.Api

`Clc.Polaris.Api` is a .NET helper library for calling the Polaris ILS PAPI REST API. It provides a typed `PapiClient` with request signing, protected/staff-override token support, and async methods for common PAPI workflows.

## Install

```bash
dotnet add package Clc.Polaris.Api --version 4.0.0-alpha.1
```

## Basic configuration

Configure `PapiClient` with your PAPI host, access credentials, workstation context, and optional staff override account:

```csharp
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;

var settings = new PapiSettings
{
    Hostname = "https://catalog.example.org",
    AccessId = "your-access-id",
    AccessKey = "your-access-key",
    OrganizationId = 100,
    WorkstationId = 1,
    UserId = 1,
    PolarisOverrideAccount = new PolarisUser("LIBRARY", "staff.user", "staff-password")
};

var client = new PapiClient(settings);
```

When `PapiClient.StaffOverrideAccount` is configured directly or through `PapiSettings.PolarisOverrideAccount`, protected requests and eligible staff-override requests can acquire protected tokens automatically. Endpoints that require a protected token in the URL path use `ProtectedToken.Placeholder` internally; callers usually do not need to supply it themselves.

## Basic async usage

Public client methods are async-only in 4.0. Await them instead of using synchronous wrappers, and pass a `CancellationToken` where your application can cancel work:

```csharp
using Clc.Polaris.Api.Models;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

var response = await client.BibKeywordSearchAsync(
    keyword: "octavia butler",
    branchId: 100,
    page: 1,
    pageSize: 10,
    cancellationToken: cts.Token);

if (response.IsSuccessStatusCode)
{
    var results = response.Data;
    // Use the search results.
}
```

## Local testing

Restore and run the test project from the repository root:

```bash
dotnet restore src/polaris-api-csharp.sln
dotnet test src/polaris-api-csharp.sln
```

Some integration-style tests may require local configuration or environment variables for a reachable Polaris PAPI instance.

## Migration guide

See [MIGRATION.md](MIGRATION.md) for 3.x to 4.0 migration notes, including async-only API changes and protected-token path behavior.
