# Polaris API C# tests

The test project contains normal unit/characterization tests plus opt-in live integration tests for the Polaris PAPI client. Normal CI excludes tests marked `TestCategory=Integration`.

## Running tests

Run only non-integration tests (the default CI behavior):

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory!=Integration"
```

Run live integration tests:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

Run live integration tests in Release configuration:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --configuration Release --filter "TestCategory=Integration"
```

If live configuration is absent, integration tests call `Assert.Inconclusive(...)` before making PAPI calls instead of throwing null-reference or configuration-binding failures.

## Configuration file setup

Copy the example file and replace placeholders with values from a safe test Polaris environment:

```bash
cp src/polaris-api-csharpTests/appsettings.Test.example.json src/polaris-api-csharpTests/appsettings.Test.json
```

`appsettings.Test.json` is intentionally not committed. Do not commit live barcodes, patron IDs, branch IDs, staff credentials, access IDs, access keys, or other fixture data.

## Environment variable setup

The integration fixture also loads environment variables using .NET configuration conventions. These are preferred in CI and for sensitive values:

```bash
export PapiSettings__Hostname="https://your-papi.example.org"
export PapiSettings__AccessId="..."
export PapiSettings__AccessKey="..."
export PapiSettings__OrganizationId="1"
export PapiSettings__UserId="1"
export PapiSettings__WorkstationId="1"
export PapiSettings__PolarisOverrideAccount__Domain="..."
export PapiSettings__PolarisOverrideAccount__Username="..."
export PapiSettings__PolarisOverrideAccount__Password="..."
export TestSettings__PatronId="12345"
export TestSettings__PatronBarcode="..."
export TestSettings__PatronPin="..."
export TestSettings__BibId="12345"
export TestSettings__BranchId="1"
export TestSettings__OrgEmail="test@example.org"
export IntegrationTestOptions__EnableMutatingIntegrationTests="false"
export IntegrationTestOptions__EnableStaffProtectedTests="false"
export IntegrationTestOptions__EnableScenarioDependentTests="false"
export IntegrationTestOptions__EnableAuthenticationFailureTests="false"
```

## Fixture data required

Minimum live smoke tests require `PapiSettings:Hostname`, `PapiSettings:AccessId`, and `PapiSettings:AccessKey`.

Additional settings unlock specific groups:

| Fixture setting | Used by |
| --- | --- |
| `TestSettings:PatronBarcode` and `TestSettings:PatronPin` | Patron authentication, patron reads, account reads, hold/item known-error reachability, title-list known-error reachability |
| `TestSettings:PatronId` | Patron ID lookup, patron search, hold create known-error reachability, notification update reachability |
| `TestSettings:BibId` | Bibliographic get, holdings, synch bib success-path smoke tests |
| `TestSettings:BranchId` / `OrganizationId` | Branch-scoped catalog, lookup table, hold-list, pickup-branch, and remote-storage tests |
| `TestSettings:OrgEmail` | `SA_GetValueByOrgAsync("ORGEMAIL")` assertion |
| `PapiSettings:PolarisOverrideAccount` | Staff/protected API tests when staff-protected tests are explicitly enabled |

## Optional gated test categories

All live tests are marked `TestCategory=Integration`. Some integration tests are additionally gated at runtime:

- Mutating tests: set `IntegrationTestOptions__EnableMutatingIntegrationTests=true`. These tests may create patron blocks, create/delete patron title lists, create/deposit account credit, update patron data, update patron notes, or exercise item barcode update behavior.
- Staff/protected tests: set `IntegrationTestOptions__EnableStaffProtectedTests=true` and provide `PapiSettings__PolarisOverrideAccount__*`. These tests authenticate staff and call protected routes.
- Scenario-dependent tests: set `IntegrationTestOptions__EnableScenarioDependentTests=true` only after configuring safe site-specific data. Current placeholders cover remote storage, renew-all, patron username update, and patron registration flows.
- Authentication-failure tests: disabled by default. Repeated bad patron PIN or staff password attempts can lock real accounts, so failed-auth behavior should only be tested with an explicit disposable fixture.

## Why some tests expect PAPI errors

Polaris PAPI often returns HTTP 200 for requests that reached the API but failed domain validation. The integration tests therefore assert the deserialized PAPI result and `PAPIErrorCode` instead of relying only on HTTP status. Known-error reachability tests use invalid/nonexistent IDs (for example record-set, hold-request, bib, item, or transaction IDs) to confirm routing, authentication/signing, response deserialization, and stable error-code behavior without mutating real records.

## Polaris API Reference Guide 8.0

The Polaris API Reference Guide 8.0 was used as the external reference for route shapes, response models, and documented negative `PAPIErrorCode` behavior for representative known-error tests. The guide is referenced by URL rather than committed because it is vendor documentation and should not be stored in this repository:

<https://knowledge.ag-software.clarivate.com/polaris/Attachments/PAPI/PolarisAPIReferenceGuide_8.0.pdf>

When the guide and client implementation differ, tests follow the current client method signatures and document scenario-dependent gaps rather than inventing fixture data.

## Integration-test coverage map

| Endpoint group | Success tests | Known-error reachability tests | Mutating tests gated by config | Scenario placeholders |
| --- | --- | --- | --- | --- |
| API/version/authentication | API key validate, API version, patron auth, staff auth | None by default | None | Failed-auth test disabled unless explicitly enabled |
| Bibliographic/catalog lookup | Bib get, branch bib get, bib search, keyword search, boolean search, holdings, synch bib | Invalid bib get | None | `HeadingsSearchAsync` remains a client `NotImplementedException` check |
| Collections/material/status lookup | Collections, dates closed, item statuses, limit filters, MARC type of materials, material types, organizations, patron codes, pickup branches, shelf locations | None | None | None |
| Patron reads | Barcode lookup, validate, basic data, preferences, circulate/renew blocks, search, holds, ILL, items out, messages, reading history, saved searches | None | None | None |
| Patron account/title lists | Account get, title-list get, create/read/delete title-list flow | Invalid pay, pay-all, refund, void, invalid title-list operations | Create credit, deposit credit, create/delete title list | None |
| Hold requests | Hold list | Invalid cancel/create/reactivate/reply/suspend/update-pickup-branch | None | None |
| Item circulation | None | Invalid item renew | Item barcode update with invalid item | Renew-all requires a patron with safely renewable checked-out items |
| Record sets/staff/protected/synch | Synch bib, SA get value when configured, notification queue when protected tests are enabled | Invalid record-set add/remove/put/get, notification update invalid payload | Patron blocks, patron no-op update, patron notes | Remote storage, patron username update, patron registration v1/v2 require site-specific fixture data |

## Refactoring note

The former monolithic `Methods/PapiClientTests.cs` mixed safe success calls, known-error reachability calls, protected calls, and mutating tests with hard-coded site IDs. It has been replaced by focused integration classes under `Integration/` plus shared infrastructure under `Integration/Infrastructure/`.
