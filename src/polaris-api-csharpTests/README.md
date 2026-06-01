# Test project

This project contains fast unit/characterization tests and opt-in live integration tests for the Polaris PAPI C# client.

## Running tests

Run the default non-integration suite (this matches normal CI behavior):

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory!=Integration"
```

Run only live integration tests:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Integration"
```

Run live integration tests in Release configuration:

```bash
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --configuration Release --filter "TestCategory=Integration"
```

If live configuration is absent, integration tests call `Assert.Inconclusive(...)` before making an API request. They should compile and skip/inconclusive cleanly in local checkouts and Codex Cloud.

## Configuration file

Copy the example file and replace placeholders with values for a disposable/safe test environment:

```bash
cp src/polaris-api-csharpTests/appsettings.Test.example.json src/polaris-api-csharpTests/appsettings.Test.json
```

`appsettings.Test.json` is intentionally not committed. The test project copies it to the output directory only when the file exists. Integration-only fixture values are bound to `IntegrationScenarioSettings` from the existing `TestSettings` section so older local configuration files and environment-variable names continue to work.

## Environment variables

The integration fixture also loads environment variables using standard .NET configuration binding. Any setting in the JSON file can be overridden with double-underscore names, for example:

- `PapiSettings__Hostname`
- `PapiSettings__AccessId`
- `PapiSettings__AccessKey`
- `PapiSettings__OrganizationId`
- `PapiSettings__UserId`
- `PapiSettings__WorkstationId`
- `PapiSettings__PolarisOverrideAccount__Domain`
- `PapiSettings__PolarisOverrideAccount__Username`
- `PapiSettings__PolarisOverrideAccount__Password`
- `TestSettings__PatronId`
- `TestSettings__PatronBarcode`
- `TestSettings__PatronPin`
- `TestSettings__BibId`
- `TestSettings__BranchId`
- `TestSettings__OrganizationId`
- `TestSettings__WorkstationId`
- `TestSettings__UserId`
- `TestSettings__OrgEmail`
- `TestSettings__PatronListName`
- `TestSettings__FreeTextBlock`
- `TestSettings__RemoteStorageBranchId`
- `TestSettings__RemoteStorageStartDate`
- `TestSettings__RemoteStorageEndDate`
- `TestSettings__RecordSetId`
- `TestSettings__ItemRecordId`
- `TestSettings__ItemBarcode`
- `TestSettings__PatronAccountTransactionId`
- `TestSettings__HoldRequestId`
- `IntegrationTestOptions__EnableMutatingIntegrationTests`
- `IntegrationTestOptions__EnableStaffProtectedTests`
- `IntegrationTestOptions__EnableScenarioDependentTests`
- `IntegrationTestOptions__EnableAuthenticationFailureTests`

## Fixture data required

Minimum live read-only configuration:

- `PapiSettings:Hostname`, `AccessId`, and `AccessKey`
- `PapiSettings:OrganizationId`, `UserId`, and `WorkstationId`

Additional success-path scenarios require:

- Patron read tests: `TestSettings:PatronId`, `PatronBarcode`, and `PatronPin`
- Catalog success tests: `TestSettings:BibId`
- Branch-specific lookups: `TestSettings:BranchId`
- Staff/protected endpoints: `PapiSettings:PolarisOverrideAccount:*` plus `IntegrationTestOptions:EnableStaffProtectedTests=true`
- `SA_GetValueByOrgAsync`: `TestSettings:OrgEmail` matching the configured organization
- Remote storage: `TestSettings:RemoteStorageBranchId`, `RemoteStorageStartDate`, and `RemoteStorageEndDate`, plus scenario-dependent tests enabled
- Optional scenario fixtures for local extensions: `TestSettings:RecordSetId`, `ItemRecordId`, `ItemBarcode`, `PatronAccountTransactionId`, and `HoldRequestId`

Do not use production patrons/items unless the site owner has explicitly approved the tests. Prefer disposable patrons, fixture records, and non-production PAPI instances. Any live mutating scenario must use disposable configured fixtures; hard-coded invalid IDs are not a safety mechanism for endpoints that can change data.

## Mutating tests

Mutating tests are disabled by default. To run them, set:

```bash
IntegrationTestOptions__EnableMutatingIntegrationTests=true
```

These tests are clearly named with `WhenMutatingTestsEnabled` when they have an executable fixture-backed flow, and they call the shared mutating gate before changing data. The mutating gate validates base PAPI configuration and requires `IntegrationTestOptions:EnableMutatingIntegrationTests=true`; patron-specific mutating tests also require patron credentials, and staff/protected mutating tests also require staff/protected credentials. Default integration runs must not call mutating endpoints. Executable mutating tests are intended for disposable fixtures only. Some mutating tests use create/read/delete flows with cleanup; account-credit and patron-note tests still mutate configured patron data and should only be enabled in safe environments.

## Staff-protected tests

Protected/staff tests are disabled by default because they require a staff override account and may access protected PAPI routes. To run them, provide `PapiSettings:PolarisOverrideAccount` and set:

```bash
IntegrationTestOptions__EnableStaffProtectedTests=true
```

Read-only staff/protected reachability tests may use known-invalid IDs and assert documented negative `PAPIErrorCode` values. Record-set content add/remove/put endpoints mutate record-set content, so they remain inconclusive placeholders until an implementation uses `EnableStaffProtectedTests=true`, `EnableMutatingIntegrationTests=true`, staff override credentials, and disposable record-set fixture data.

## Scenario-dependent placeholders

Some client methods require real local data that cannot be safely invented, such as renewable checked-out items, complete patron registration payloads, disposable account-payment fixtures, disposable hold requests, disposable item-barcode fixtures, or disposable record sets. The suite includes inconclusive placeholder tests documenting the method name, why scenario data is required, the required fixture settings, and the intended assertion strategy. Documentation-only placeholders always remain `Assert.Inconclusive(...)` until they are replaced with a real fixture-backed implementation; setting `IntegrationTestOptions:EnableScenarioDependentTests=true` must not create false green coverage for unimplemented placeholders.

Executable scenario-dependent tests, such as remote-storage reads, still use the scenario-dependent option plus explicit fixture settings:

```bash
IntegrationTestOptions__EnableScenarioDependentTests=true
```

## Known-error reachability tests

PAPI often returns HTTP 200 with a `PAPIErrorCode` carrying endpoint-level status. Negative `PAPIErrorCode` values represent PAPI/domain errors. Zero and positive values are no-error success codes; positive values commonly represent rows returned or rows affected. The integration success helper therefore treats any non-negative `PAPIErrorCode` as success, and list/read tests keep stronger row-count assertions where the response collection has stable row-count semantics.

Known-error reachability tests assert exact negative `PAPIErrorCode` values only when the endpoint is safe to call as a read-only operation and the Polaris API Reference Guide 8.0 documents the code or the previous integration suite already used that stable code. Hard-coded invalid IDs may be acceptable for read-only known-error tests, but they are not used as the safety mechanism for mutating endpoints because a supposedly nonexistent ID could exist in a real Polaris database. Mutating operations instead require disposable configured fixtures or remain documented inconclusive placeholders. Tests avoid asserting exact `ErrorMessage` text unless it is needed and stable.

## Failed authentication tests

Failed staff login and patron PIN tests are disabled by default because repeated bad credentials can lock accounts. The checked-in suite includes a placeholder rather than repeatedly testing bad passwords against real accounts. Use only disposable credentials if you add a local failed-auth test.

## Polaris API Reference Guide 8.0

The integration suite was refactored using the Polaris API Reference Guide 8.0 as the primary endpoint/error-code reference:

<https://knowledge.ag-software.clarivate.com/polaris/Attachments/PAPI/PolarisAPIReferenceGuide_8.0.pdf>

The guide was used selectively to confirm endpoint routes, response shapes, row-count semantics, and stable negative `PAPIErrorCode` values such as invalid bib, hold request, item checkout, payment transaction, and record-set IDs. The PDF is referenced by URL instead of committed because it is vendor documentation, large, and not needed to build or run tests.

## Integration suite structure and coverage map

| Endpoint group | Success tests | Known-error reachability tests | Mutating tests gated by config | Placeholders requiring scenario data |
| --- | --- | --- | --- | --- |
| API/version/authentication | API key validation, API version, patron auth, staff auth when enabled | None by default | None | Failed-auth testing is a placeholder because account lockout is a risk |
| Bibliographic/catalog lookup | Bib get, branch-specific bib get, keyword/boolean search, holdings | None | None | `HeadingsSearchAsync` remains a client `NotImplementedException` check |
| Reference data lookup | Collections, dates closed, item statuses, limit filters, MARC types, material types, organizations, patron codes, pickup branches, shelf locations | None | None | None |
| Synch | synch bibs by ID | None | None | None |
| Patron reads | validate, barcode-from-id, basic data, blocks, hold/ILL/items out, messages, preferences, reading history, renew blocks, saved searches, patron search | None | None | None |
| Patron account/title lists | account get, title lists get | None for payment/refund/void defaults | create/delete unique title list, create credit, deposit credit | payment/refund/void and title-list mutating reachability require disposable account/list fixtures |
| Hold requests/circulation | hold request list requires staff/protected credentials | hold status by known-invalid request/GUID | None by default | hold create/cancel/reactivate/suspend/pickup-branch updates, item renew, renew-all, and item barcode update require disposable fixtures |
| Patron mutations/messages | None by default | unsafe username update asserts unauthorized | patron blocks, no-op patron update, notes update | message delete/status, reading-history clear, notification update, and patron registration v1/v2 require disposable fixtures |
| Staff/protected/record sets | record-set records get, SA value lookup, and notification queue when staff tests are enabled | known-invalid record-set get | None by default | record-set content add/remove/put require staff/protected plus mutating gates and disposable record-set fixtures; remote storage requires branch/date activity data |
