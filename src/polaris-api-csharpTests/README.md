# Polaris API C# tests

The default test command is safe for CI and excludes live Polaris PAPI calls. Live integration tests are opt-in and are all marked with `TestCategory=Integration`.

## Local configuration

Copy the placeholder file and fill it with values from a disposable Polaris/PAPI test environment:

```bash
cp src/polaris-api-csharpTests/appsettings.Test.example.json src/polaris-api-csharpTests/appsettings.Test.json
```

`appsettings.Test.json` is intentionally not committed. The test project copies it to the output directory when present.

You can also configure tests with standard .NET environment-variable keys. Useful examples:

```bash
export PapiSettings__Hostname="https://papi.example.org"
export PapiSettings__AccessId="..."
export PapiSettings__AccessKey="..."
export PapiSettings__PolarisOverrideAccount__Domain="..."
export PapiSettings__PolarisOverrideAccount__Username="..."
export PapiSettings__PolarisOverrideAccount__Password="..."
export TestSettings__PatronBarcode="..."
export TestSettings__PatronPin="..."
export TestSettings__PatronId="123"
export TestSettings__BibId="456"
export IntegrationTestOptions__EnableMutatingIntegrationTests="false"
```

Sensitive settings should normally be supplied by environment variables or repository/environment secrets rather than checked-in JSON.

## Commands

Run unit/non-integration tests only:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory!=Integration"
```

Run integration tests in the default configuration:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

Run integration tests in Release:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --configuration Release --filter "TestCategory=Integration"
```

When required live configuration is missing or still contains placeholders, tests call `Assert.Inconclusive(...)` before making a live API call.

## Fixture data

The safest integration tests require only PAPI host/access-key settings. Scenario-dependent tests additionally need configured fixture data:

| Fixture value | Used by |
| --- | --- |
| `TestSettings:PatronBarcode`, `TestSettings:PatronPin`, `TestSettings:PatronId` | Patron reads, patron account reads, hold/item known-error reachability tests |
| `TestSettings:BibId` | Bibliographic and synch success tests |
| `TestSettings:BranchId`, `TestSettings:OrganizationId` | Branch/org-specific lookup and protected methods |
| `PapiSettings:PolarisOverrideAccount:*` plus `EnableStaffProtectedTests=true` | Protected/staff endpoints |
| `PatronListName`, `FreeTextBlock` plus `EnableMutatingIntegrationTests=true` | Mutating title-list and patron-block tests |
| `RecordSetId`, `ItemRecordId`, `ItemBarcode`, `HoldRequestId`, `PatronAccountTransactionId` | Reserved for future scenario-dependent fixtures where safe setup/cleanup is environment-specific |

Do not use production patrons, items, barcodes, bibs, branches, workstations, or staff credentials for mutating tests.

## Optional categories and gates

- **Mutating tests**: set `IntegrationTestOptions:EnableMutatingIntegrationTests=true`. These tests are named clearly and use disposable data where possible. They default to inconclusive.
- **Staff/protected tests**: set `IntegrationTestOptions:EnableStaffProtectedTests=true` and configure `PapiSettings:PolarisOverrideAccount`. Protected-token tests default to inconclusive.
- **Scenario-dependent tests**: set `IntegrationTestOptions:EnableScenarioDependentTests=true` only after adding explicit safe fixture data and cleanup strategy. Placeholder tests document the required method, fixtures, and assertion strategy.
- **Authentication-failure tests**: disabled by default because repeated failed patron PIN or staff-password attempts can lock real accounts. Prefer success-path authentication tests and non-auth invalid IDs for reachability checks.

## Why some tests assert PAPI errors

PAPI often returns HTTP `200 OK` while reporting domain failures in `PAPIErrorCode`. Integration tests therefore assert deserialization and `PAPIErrorCode` instead of relying only on HTTP status. Invalid record IDs, request IDs, record-set IDs, and item IDs are used for known-error reachability tests because they are safer than repeated bad credentials.

## Polaris API Reference Guide 8.0

The suite was checked against the Polaris API Reference Guide 8.0:

<https://knowledge.ag-software.clarivate.com/polaris/Attachments/PAPI/PolarisAPIReferenceGuide_8.0.pdf>

The guide was used selectively to confirm endpoint groupings, response shapes, row-count conventions, protected-method requirements, and documented negative PAPI error-code behavior. The PDF is referenced by URL and is not committed because it is third-party documentation, is large, and may be updated independently of this repository.

## Integration coverage map

| Endpoint group | Success/smoke tests | Known-error reachability tests | Mutating tests gated by config | Placeholders requiring scenario data |
| --- | --- | --- | --- | --- |
| API/authentication | API key validation, API version, patron auth, staff auth when enabled | Failed-auth intentionally not run by default | None | Disposable failed-auth scenario |
| Bibliographic/catalog | Bib get, branch bib get, bib search, keyword search, boolean search, holdings | Nonexistent bib deserialization | None | `HeadingsSearchAsync` remains a client-side `NotImplementedException` characterization |
| Reference data/material/status | Collections, dates closed, item statuses, limit filters, MARC/material types, organizations, patron codes, pickup branches, shelf locations | None by default | None | Environment-specific assertions beyond row-count conventions |
| Patron reads | Validate, barcode-from-id, basic data, circulate blocks, holds, ILL, items out, messages, preferences, reading history, renew blocks, saved searches, patron search | None by default | None | Patrons with specific account/history states if stricter row assertions are desired |
| Patron account/title lists | Account get, title-list get | Invalid pay/pay-all/refund/void and invalid title-list ids | Credit/deposit and create/get/delete title-list flow | Real account transaction lifecycle fixtures |
| Hold requests | None without scenario state | Invalid cancel/create/reactivate/reply/suspend/update-pickup-branch ids | None | Real hold creation/cancel/reply lifecycle fixture |
| Item circulation | None without checked-out item state | Invalid item renew and item barcode update | None | Renewable checked-out item for renew-all |
| Patron mutations/messages | None by default | Invalid notification, patron message, reading-history, patron-note scenarios | Free-text block and empty patron update when explicitly enabled | Patron username update and patron registration V1/V2 |
| Record sets/staff | SA value, remote storage smoke when staff enabled | Invalid record-set add/remove/get | None by default | Disposable record set add/remove verification |
| Synch | Synch bibs by configured bib id | Invalid bib deserialization | None | Broader synch endpoint fixture expansion |
