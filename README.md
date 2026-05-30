# Clc.Polaris.Api

`Clc.Polaris.Api` is a .NET helper library for calling the Polaris ILS PAPI REST API. It provides a small `PapiClient` wrapper, typed request/response models, PAPI request signing, and protected/staff-override token handling.

## Install

Install the `4.0.0-alpha.1` prerelease package:

```bash
dotnet add package Clc.Polaris.Api --version 4.0.0-alpha.1
```

## Configure `PapiClient`

Create settings with your Polaris PAPI credentials and pass them to `PapiClient`:

```csharp
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;

var settings = new PapiSettings
{
    Hostname = "https://polaris.example.org",
    AccessId = "your-access-id",
    AccessKey = "your-access-key",
    OrganizationId = 3,
    WorkstationId = 12,
    UserId = 42,
    PolarisOverrideAccount = new PolarisUser("LIBRARY", "staff.user", "password")
};

var client = new PapiClient(settings);
```

When `PolarisOverrideAccount` / `StaffOverrideAccount` is configured, protected requests and eligible staff-override requests can acquire protected tokens automatically. Endpoints that require a protected token in the URL path use `ProtectedToken.Placeholder` internally; callers normally do not need to set or replace that placeholder themselves.

## Basic async usage

All public client methods are async-only. Await the `*Async` method and pass a `CancellationToken` where appropriate so the PAPI request and any required authentication request can be cancelled.

```csharp
using Clc.Polaris.Api.Models;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

var response = await client.BibKeywordSearchAsync(
    keyword: "octavia butler",
    branchId: 3,
    page: 1,
    pageSize: 10,
    cancellationToken: cts.Token);

if (response.Response.IsSuccessStatusCode && response.Data?.BibSearchRows != null)
{
    foreach (var bib in response.Data.BibSearchRows)
    {
        Console.WriteLine($"{bib.Title} ({bib.ControlNumber})");
    }
}
```

Do not block on client calls with synchronous wrappers or `.Result`; synchronous public client APIs are not supported in 4.0.

## Local testing

From the repository root, run:

```bash
dotnet test src/polaris-api-csharp.sln
```

## Migration guide

See [MIGRATION.md](MIGRATION.md) for guidance on moving from the 3.x synchronous API to the 4.0 async-only API.
