# Clc.Polaris.Api

`Clc.Polaris.Api` is a .NET helper package for calling the Polaris Integrated Library System (ILS) PAPI REST API from C# applications.

## Install

```bash
dotnet add package Clc.Polaris.Api --version 4.0.0-beta.1
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

`PapiSettings.PolarisOverrideAccount` is copied to `PapiClient.StaffOverrideAccount`. When configured, the client can acquire protected tokens automatically for protected methods and public patron-account methods that support staff override. General public methods continue to use normal PAPI signing and do not receive staff override credentials.

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

Run unit tests from the repository root:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Unit"
```

Integration tests require a live Polaris dev environment plus local dev credentials and test data in `src/polaris-api-csharpTests/appsettings.Test.json`, so they are not run by default in CI. Keep this file local only: do not commit real secrets, and remember that `appsettings.Test.json` is ignored by git.

The live-test categories are mutually exclusive so each live test appears under exactly one trait in Visual Studio Test Explorer:

- `ReadOnlyIntegration`: live test that only reads/returns data and does not mutate Polaris state.
- `ProtectedReadOnlyIntegration`: read-only live test that requires staff/protected credentials.
- `MutatingIntegration`: live test that creates, updates, cancels, deletes, clears, pays, voids, moves, renews, submits, or otherwise changes Polaris state.
- `ProtectedMutatingIntegration`: mutating live test that requires staff/protected credentials.

Run read-only integration tests that do not require staff credentials when you have local Polaris dev settings configured:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=ReadOnlyIntegration"
```

Run protected read-only integration tests only when `PapiSettings.PolarisOverrideAccount` is configured with staff override credentials:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=ProtectedReadOnlyIntegration"
```

Run all read-only integration tests when both standard and staff/protected local credentials are available:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=ReadOnlyIntegration|TestCategory=ProtectedReadOnlyIntegration"
```

Run mutating/destructive integration tests only against a disposable Polaris dev environment that is refreshed nightly or otherwise safe to dirty:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=MutatingIntegration|TestCategory=ProtectedMutatingIntegration"
```

`MutatingIntegration` and `ProtectedMutatingIntegration` tests may create or modify patron blocks, notes, title lists, account entries, record-set entries, hold requests, pickup-branch values, or similar artifacts in Polaris. These artifacts may be left behind; cleanup is not guaranteed. Same-day collision avoidance is handled by unique test names and notes, not by assuming prior artifacts were removed.

The required baseline settings below are enough for the basic live integration tests. The nullable optional IDs enable broader disposable-environment lifecycle coverage. Tests that need an optional ID call `Assert.Inconclusive` with a targeted message when that ID is not configured, rather than making the whole integration suite require that data.

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
  "OrgEmail": "library@example.org",
  "BibId": 478907,
  "BranchId": 7,
  "PickupBranchId": 7,
  "LocalControlNumber": 478907,
  "RecordSetId": 12345,
  "RecordSetRecordId": 478907,
  "HoldableBibId": 478907,
  "HoldPickupBranchId": 7,
  "StaffUserId": 1,
  "StaffWorkstationId": 1
}
```

Optional ID usage:

- `BibId`, `BranchId`, `PickupBranchId`, and `LocalControlNumber` drive success-path bibliographic lookups, holdings, branch lookup, and title-list title operations.
- `HoldableBibId`, `HoldableItemRecordId`, and `HoldPickupBranchId` identify disposable hold test data; omit them unless the patron can safely create/cancel holds for that title or item.
- `RecordSetId`, `RecordSetRecordId`, `StaffUserId`, and `StaffWorkstationId` enable protected record-set and staff-attributed account/note/hold scenarios.

Run the full live integration suite manually when disposable Polaris credentials and optional data are available:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --configuration Release --filter "TestCategory=ReadOnlyIntegration|TestCategory=ProtectedReadOnlyIntegration|TestCategory=MutatingIntegration|TestCategory=ProtectedMutatingIntegration" --logger trx --results-directory TestResults
```

## Migration guide

See [MIGRATION.md](MIGRATION.md) for 3.x-to-4.0 async migration notes.
