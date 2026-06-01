# Polaris API C# tests

The test project contains fast unit/characterization tests and opt-in live integration tests for the Polaris PAPI client.
Normal CI must continue to exclude integration tests:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory!=Integration"
```

## Live integration configuration

Integration tests load configuration from an optional `appsettings.Test.json` file and then from environment variables. The real file is intentionally not committed.

```bash
cp src/polaris-api-csharpTests/appsettings.Test.example.json src/polaris-api-csharpTests/appsettings.Test.json
# edit appsettings.Test.json with live test-system values only
```

Sensitive settings can be supplied with .NET configuration environment variable names instead of JSON, for example:

- `PapiSettings__Hostname`
- `PapiSettings__AccessId`
- `PapiSettings__AccessKey`
- `PapiSettings__PolarisOverrideAccount__Domain`
- `PapiSettings__PolarisOverrideAccount__Username`
- `PapiSettings__PolarisOverrideAccount__Password`
- `TestSettings__PatronBarcode`
- `TestSettings__PatronPin`
- `IntegrationTestOptions__EnableMutatingIntegrationTests`
- `IntegrationTestOptions__EnableStaffProtectedTests`
- `IntegrationTestOptions__EnableScenarioDependentTests`

When required PAPI settings are absent, tests call `Assert.Inconclusive(...)` before making live API calls. Placeholder values from `appsettings.Test.example.json` are treated as missing.

## Running tests

Run only non-integration tests:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory!=Integration"
```

Run integration tests with Debug defaults:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

Run integration tests the same way as release verification:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --configuration Release --filter "TestCategory=Integration"
```

## Fixture data and opt-in flags

The live suite uses three styles of tests:

1. **Safe success tests** for read-only endpoints such as API validation, version, catalog lookup, patron reads, and lookup tables.
2. **Known-error reachability tests** that intentionally use invalid/nonexistent IDs to prove the request reaches PAPI and the response deserializes. PAPI often returns HTTP 200 with a negative `PAPIErrorCode` for domain failures, so these tests assert the PAPI response body rather than only HTTP status.
3. **Gated mutation/scenario tests** that are inconclusive unless explicit flags and fixture data are supplied.

Required fixture settings vary by group:

| Setting | Used by |
| --- | --- |
| `PapiSettings:Hostname`, `AccessId`, `AccessKey` | every live API call |
| `TestSettings:PatronId`, `PatronBarcode`, `PatronPin` | patron success tests, patron account tests, holds, item renewal |
| `TestSettings:BibId`, `BranchId` | catalog success tests and branch-scoped lookup tests |
| `PapiSettings:PolarisOverrideAccount:*` | staff/protected tests |
| `TestSettings:RecordSetId` | record-set success scenario tests |
| `TestSettings:ItemRecordId`, `ItemBarcode` | reversible item barcode update scenario |

### Mutating tests

Mutating tests are disabled by default. Enable them only against a dedicated test system and fixture records:

```bash
IntegrationTestOptions__EnableMutatingIntegrationTests=true \
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

### Staff/protected tests

Protected endpoints and staff authentication are disabled by default because they require staff credentials:

```bash
IntegrationTestOptions__EnableStaffProtectedTests=true \
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

### Scenario-dependent tests

Some endpoints require specific existing data (for example record sets or remote-storage activity). Enable these only after configuring the matching fixture values:

```bash
IntegrationTestOptions__EnableScenarioDependentTests=true \
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

### Failed authentication tests

Failed patron/staff authentication tests are disabled by default because repeated bad PINs or passwords can lock real accounts. Prefer success-path authentication tests with a dedicated patron/staff fixture. Only enable failure testing against a lockout-safe fixture and keep attempts minimal.

## Polaris API Reference Guide 8.0

The integration suite was checked against the Polaris API Reference Guide 8.0 URL:

<https://knowledge.ag-software.clarivate.com/polaris/Attachments/PAPI/PolarisAPIReferenceGuide_8.0.pdf>

The guide was used selectively to confirm that negative `PAPIErrorCode` values represent domain-level errors, to verify several documented success/error examples (for example `BibGet`, hold request, item renewal, and record set behavior), and to separate success-path tests from known-error reachability tests. The PDF is referenced by URL and is not committed because it is external vendor documentation and should not become repository test fixture data.

## Endpoint coverage summary

| Endpoint group | Success tests | Known-error reachability tests | Mutating tests gated by config | Scenario placeholders |
| --- | --- | --- | --- | --- |
| API/version/authentication | API key validation, API version, patron auth success, staff auth gated | failed auth intentionally not run by default | none | guarded failed-auth placeholder |
| Bibliographic/catalog lookup | configured `BibGet`, search, collections, holdings, sync | nonexistent `BibGet` | none | configured bib/branch required |
| Collections/material/status lookup | dates closed, organizations, pickup branches, shelves, material types, item statuses, limit filters | none | none | branch/org required |
| Patron reads | basic data, validate, barcode lookup, blocks, ILL/items/messages/preferences/history/saved searches/search | none | none | configured patron required |
| Patron account/title lists | account get | payments/void/refund with nonexistent transaction/list IDs | credit/deposit and create/get/delete title-list flow | none |
| Hold requests | request list | nonexistent request/bib hold actions | none | configured patron/branch required |
| Item circulation | none | renew nonexistent item | item barcode update placeholder gated | item barcode reversible fixture required |
| Record sets | optional configured record-set read | nonexistent record-set content add/remove/get | content changes only against nonexistent IDs by default | record-set success fixture required |
| Staff/protected/synch | protected queue and SA value gated, bib sync | notification update nonexistent/default error | patron blocks/notes gated | staff credentials required |
| Patron registration/remote storage | none by default | remote storage allows documented no-data style response when scenario-enabled | registration placeholder only | site-specific registration and remote-storage data required |
