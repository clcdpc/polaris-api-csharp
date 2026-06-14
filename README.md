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

The test suite is split into two MSTest projects:

- `tests/Clc.Polaris.Api.UnitTests/Clc.Polaris.Api.UnitTests.csproj` contains deterministic tests that are safe to run on every PR, including request-shape, route, query-string, header, cancellation, validation, protected-token substitution, authorization-hash, model, and coverage-governance tests.
- `tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj` contains tests that call a real Polaris environment and require credentials, configured test IDs, and/or disposable state.

Run unit tests from the repository root:

```bash
dotnet test tests/Clc.Polaris.Api.UnitTests/Clc.Polaris.Api.UnitTests.csproj
```

Live integration tests require a live Polaris dev environment plus local dev credentials and test data in `tests/Clc.Polaris.Api.LiveIntegrationTests/appsettings.Test.json` or equivalent environment variables, so they are not run by default in CI. Keep this file local only: do not commit real secrets, and remember that `appsettings.Test.json` is ignored by git.

The live-test category taxonomy is composable:

- `ReadOnly`: live test that only reads/returns data and does not intentionally mutate Polaris state.
- `Mutating`: live test that creates, updates, cancels, deletes, clears, pays, voids, moves, renews, submits, or otherwise changes Polaris state.
- `RequiresStaffOverride`: live test that requires staff override/protected credentials.
- `RequiresDisposableData`: live test that should only run against disposable or safe-to-dirty test data.
- `Lifecycle`: live test that exercises a multi-step workflow across related operations.
- `Smoke`: small, low-risk read-only subset for quick environment validation.

Run all live integration tests when the configured Polaris environment is safe for both read-only and mutating scenarios:

```bash
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --configuration Release --filter "TestCategory=ReadOnly|TestCategory=Mutating" --logger trx --results-directory TestResults
```

Useful live-test filters:

```bash
# Read-only only
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --filter "TestCategory=ReadOnly"

# Staff override/protected credentials only
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --filter "TestCategory=RequiresStaffOverride"

# Mutating/disposable data only
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --filter "TestCategory=Mutating"

# Lifecycle workflows only
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --filter "TestCategory=Lifecycle"

# Smoke only
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --filter "TestCategory=Smoke"
```

`Mutating` tests may create or modify patron blocks, notes, title lists, account entries, record-set entries, hold requests, pickup-branch values, or similar artifacts in Polaris. These artifacts may be left behind; cleanup is not guaranteed. Same-day collision avoidance is handled by unique test names and notes, not by assuming prior artifacts were removed. Only run mutating tests against a disposable Polaris dev environment that is refreshed nightly or otherwise safe to dirty.

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
dotnet test tests/Clc.Polaris.Api.LiveIntegrationTests/Clc.Polaris.Api.LiveIntegrationTests.csproj --configuration Release --filter "TestCategory=ReadOnly|TestCategory=Mutating" --logger trx --results-directory TestResults
```

## Migration guide

See [MIGRATION.md](MIGRATION.md) for 3.x-to-4.0 async migration notes.
