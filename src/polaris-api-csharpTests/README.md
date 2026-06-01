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

`appsettings.Test.json` is intentionally not committed. The test project copies it to the output directory only when the file exists. Integration-only fixture values are bound to `IntegrationScenarioSettings` from the `TestSettings` section. For compatibility with pre-refactor local files, the fixture also binds legacy root-level settings first (for example `PatronId`, `PatronBarcode`, `PatronPin`, `FreeTextBlock`, `PatronListName`, and `OrgEmail`) and then lets the nested `TestSettings` section override them when it exists, so older local configuration files and environment-variable names continue to work.

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

When using the manual GitHub integration workflow, omit optional numeric fixture secrets that are not configured instead of setting them to empty strings. The workflow writes numeric `PapiSettings__...` and `TestSettings__...` variables only when the corresponding secret has a value, which avoids .NET configuration binding failures from empty strings before test gates can mark missing config inconclusive.

## Fixture data required

Minimum live read-only configuration:

- `PapiSettings:Hostname`, `AccessId`, and `AccessKey`
- `PapiSettings:OrganizationId`, `UserId`, and `WorkstationId`

Additional success-path scenarios require:

- Patron read tests: `TestSettings:PatronId`, `PatronBarcode`, and `PatronPin`
- Catalog success tests: `TestSettings:BibId`
- Branch-specific lookups: `TestSettings:BranchId`
- Staff/protected endpoints, including protected patron/synch read routes such as barcode-from-ID, renew-blocks, patron search, and synch bibs: `PapiSettings:PolarisOverrideAccount:*` plus `IntegrationTestOptions:EnableStaffProtectedTests=true`
- `SA_GetValueByOrgAsync`: `TestSettings:OrgEmail` matching the configured organization
- Remote storage: `TestSettings:RemoteStorageBranchId`, `RemoteStorageStartDate`, and `RemoteStorageEndDate`, plus scenario-dependent tests enabled
- Optional scenario fixtures for local extensions: `TestSettings:RecordSetId`, `ItemRecordId`, `ItemBarcode`, `PatronAccountTransactionId`, and `HoldRequestId`

Do not use production patrons/items unless the site owner has explicitly approved the tests. Prefer disposable patrons, fixture records, and non-production PAPI instances.

## Mutating tests

Mutating tests are disabled by default, and default integration runs should not call mutating endpoints. To run implemented mutating tests, set:

```bash
IntegrationTestOptions__EnableMutatingIntegrationTests=true
```

The mutating gate checks live PAPI configuration and the explicit option only. Patron-specific mutating tests also require `TestSettings:PatronBarcode` and `TestSettings:PatronPin`; staff/protected mutating tests must also satisfy the staff/protected gate. These tests are intended for disposable fixtures only. Some mutating tests use create/read/delete flows with cleanup; account-credit, patron-block, patron-update, and patron-note tests still mutate configured patron data and should only be enabled in safe environments. Hard-coded invalid IDs are not used as safety mechanisms for mutating operations; any live mutating scenario that needs item, record-set, account, hold, notification, or title-list data must use explicitly configured disposable fixtures.

## Staff-protected tests

Protected/staff tests are disabled by default because they require a staff override account and may access protected PAPI routes. To run them, provide `PapiSettings:PolarisOverrideAccount` credentials and set:

```bash
IntegrationTestOptions__EnableStaffProtectedTests=true
```

Read-only staff/protected reachability tests may use known-invalid sentinel IDs and assert documented negative `PAPIErrorCode` values. Protected patron and synch read endpoints also require staff override credentials and `IntegrationTestOptions:EnableStaffProtectedTests=true`, even when they use patron or bib fixture data. Record-set content add/remove/put tests are mutating and remain inconclusive placeholders until disposable record-set fixtures are configured and an executable cleanup strategy is implemented.

## Scenario-dependent placeholders

Some client methods require real local data that cannot be safely invented, such as renewable checked-out items, complete patron registration payloads, payment/refund/void fixtures, disposable item barcode fixtures, hold fixtures, record-set mutation fixtures, or remote-storage activity windows. Placeholder tests always call `Assert.Inconclusive(...)` and do not pass merely because `IntegrationTestOptions:EnableScenarioDependentTests=true`; they document the method name, why scenario data is required, the required fixture settings, and the intended assertion strategy until a fixture-backed implementation exists.

Executable scenario-dependent tests may still use this option as an additional opt-in gate after validating all required fixture settings:

```bash
IntegrationTestOptions__EnableScenarioDependentTests=true
```

## Known-error reachability tests

PAPI often returns HTTP 200 with a `PAPIErrorCode` carrying endpoint-level status. Negative `PAPIErrorCode` values represent PAPI/domain errors. Zero and positive values are no-error success codes; positive values commonly represent rows returned or rows affected. The integration success helper therefore treats any non-negative `PAPIErrorCode` as success, and list/read tests keep stronger row-count assertions where the response collection has stable row-count semantics.

Read-only known-error reachability tests may use high known-invalid sentinel IDs to exercise the route, authentication, deserialization, and documented PAPI error-code behavior without relying on fragile success state. Mutating endpoints must not rely on hard-coded nonexistent IDs as their safety mechanism; they either require explicit disposable configured fixtures and mutating opt-in gates or remain inconclusive scenario-dependent placeholders. Known-error reachability tests still assert exact negative `PAPIErrorCode` values only when the Polaris API Reference Guide 8.0 documents the code or the previous integration suite already used that stable code. They avoid asserting exact `ErrorMessage` text unless it is needed and stable.

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
| Patron account/title lists | account get, title lists get | None by default for payment/refund/void/title-list mutations | unique create/delete title list, create credit, deposit credit | payment/refund/void and hard-coded title-list mutation reachability require disposable account/list fixtures |
| Hold requests/circulation | hold request list (staff/protected-gated) | None by default for hold/item mutations | None without disposable fixtures | hold create/cancel/reactivate/suspend/reply/pickup updates, item renew, renew-all, and item barcode update require disposable fixtures |
| Patron mutations/messages | None by default | None by default for patron message/reading-history/notification mutations | patron blocks, no-op patron update, username unauthorized check, notes update | patron message, reading-history, notification update, and registration v1/v2 require disposable fixtures |
| Staff/protected/record sets | hold request list, SA value lookup, notification queue when staff tests are enabled | read-only record-set get with a known-invalid sentinel ID | None without disposable fixtures | record-set add/remove/put mutations and remote storage require explicit scenario data |
