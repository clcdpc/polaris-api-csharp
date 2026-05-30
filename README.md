# Clc.Polaris.Api

`Clc.Polaris.Api` is a .NET helper library for calling the Polaris ILS PAPI REST API. It provides a configured `PapiClient`, request signing, strongly typed request/response models, and async public client methods for common public and protected PAPI operations.

## Install

```bash
dotnet add package Clc.Polaris.Api --version 4.0.0-alpha.1
```

## Configure `PapiClient`

Create a `PapiSettings` instance with your PAPI host and credentials, then pass it to `PapiClient`.

```csharp
using Clc.Polaris.Api;
using Clc.Polaris.Api.Configuration;
using Clc.Polaris.Api.Models;

var settings = new PapiSettings
{
    Hostname = "https://polaris.example.org",
    AccessId = "your-access-id",
    AccessKey = "your-access-key",
    OrganizationId = 1,
    UserId = 123,
    WorkstationId = 456,
    PolarisOverrideAccount = new PolarisUser("STAFF", "api-user", "staff-password")
};

using var httpClient = new HttpClient();
var client = new PapiClient(httpClient, settings);
```

When `PolarisOverrideAccount` is configured, protected requests and eligible staff-override requests can automatically acquire a protected token before sending the PAPI request.

## Basic async usage

Public client methods are async-only in 4.0. Await calls and pass a `CancellationToken` where cancellation or request timeouts should be controlled by the caller.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

var response = await client.BibKeywordSearchAsync(
    keyword: "octavia butler",
    branchId: settings.OrganizationId,
    page: 1,
    pageSize: 10,
    cancellationToken: cts.Token);

if (response.IsSuccessful)
{
    var results = response.Data;
    // Use results here.
}
```

Protected-token-in-path endpoints use `ProtectedToken.Placeholder` internally. `PapiClient` replaces the placeholder with the acquired protected access token before the request is sent; callers generally do not need to use it directly.

## Local tests

Run the test suite from the repository root:

```bash
dotnet test
```

For tests or local integrations that require live Polaris credentials, copy `src/polaris-api-csharpTests/appsettings.template.json` to a local settings file as appropriate for your environment and keep secrets out of source control.

## Migration

See [MIGRATION.md](MIGRATION.md) for 3.x to 4.0 async migration notes and examples.
