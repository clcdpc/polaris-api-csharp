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

`appsettings.Test.json` is intentionally not committed. The test project copies it to the output directory only when the file exists. Integration-only fixture values are bound to `IntegrationScenarioSettings` from the `TestSettings` section when that section exists. Legacy local files that kept fixture fields such as `PatronId`, `PatronBarcode`, `PatronPin`, `FreeTextBlock`, `PatronListName`, and `OrgEmail` at the JSON root are still supported as a fallback, and `TestSettings__...` environment variables continue to override those root-level values.

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


When configuring GitHub Actions secrets or local environment variables, omit optional numeric fixture values that are not configured instead of setting them to empty strings. Empty strings cannot be converted to integers by .NET configuration binding; omitted values remain at their safe default of `0` and the relevant integration tests call `Assert.Inconclusive(...)` when they need fixture data.

## Fixture data required

Minimum live read-only configuration:

- `PapiSettings:Hostname`, `AccessId`, and `AccessKey`
- `PapiSettings:OrganizationId`, `UserId`, and `WorkstationId`

Additional success-path scenarios require:

- Patron read tests: `TestSettings:PatronId`, `PatronBarcode`, and `PatronPin`
- Catalog success tests: `TestSettings:BibId`
- Branch-specific lookups: `TestSettings:BranchId`
- Staff/protected endpoints, including protected read routes such as patron barcode-from-ID, patron renew-blocks, patron search, synch bibs, hold request list, record-set reads, SA value lookup, notification queue, and remote storage: `PapiSettings:PolarisOverrideAccount:*` plus `IntegrationTestOptions:EnableStaffProtectedTests=true`
- `SA_GetValueByOrgAsync`: `TestSettings:OrgEmail` matching the configured organization
- Remote storage: `TestSettings:RemoteStorageBranchId`, `RemoteStorageStartDate`, and `RemoteStorageEndDate`, plus scenario-dependent tests enabled
- Optional scenario fixtures for local extensions: `TestSettings:RecordSetId`, `ItemRecordId`, `ItemBarcode`, `PatronAccountTransactionId`, and `HoldRequestId`

Do not use production patrons/items unless the site owner has explicitly approved the tests. Prefer disposable patrons, fixture records, and non-production PAPI instances.

## Mutating tests

Mutating tests are disabled by default, and default integration runs should not call mutating endpoints. To run implemented mutating tests, set:

```bash
IntegrationTestOptions__EnableMutatingIntegrationTests=true
```

Executable mutating tests are grouped by API surface instead of being combined into a single cross-surface orchestration test. Each surface must own its fixture requirements, cleanup path, and assertions. The mutating gate checks live PAPI configuration and the explicit option only. Patron-specific mutating tests also require `TestSettings:PatronBarcode` and `TestSettings:PatronPin`; staff/protected mutating tests must also satisfy the staff/protected gate. These tests are intended for disposable fixtures only.

The title-list mutating workflow creates only uniquely named disposable title lists for the current test run. It finds generated lists by exact generated name, registers cleanup immediately after each successful create, exercises title-list content operations only inside those generated lists when `TestSettings:BibId` is configured, and deletes generated lists in reverse order during `finally`. It must not touch pre-existing title lists.

Account-credit endpoints such as create-credit and deposit-credit mutate durable patron account state. They should become executable only when the client/model surface can uniquely identify the generated account transaction or credit row and reliably reverse that same artifact, for example with an appropriate refund or void operation. Until that cleanup path is reliable, account-credit coverage remains an inconclusive placeholder and makes no live API call.

Patron blocks and patron notes remain durable mutating tests because there is no reliable cleanup path in the current integration suite. Run them only against disposable patrons in safe environments. No mutating endpoint should rely on hard-coded nonexistent IDs as a safety mechanism; any live mutating scenario that needs item, record-set, account, hold, notification, or title-list data must use explicitly configured disposable fixtures and a documented cleanup or durability strategy.

## Staff-protected tests

Protected/staff tests are disabled by default because they require a staff override account and may access protected PAPI routes. To run them, provide `PapiSettings:PolarisOverrideAccount` credentials and set:

```bash
IntegrationTestOptions__EnableStaffProtectedTests=true
```

Read-only staff/protected tests include protected patron, synch, circulation, record-set, SA, notification, and remote-storage routes; they must satisfy this gate even when they also require patron or scenario fixture settings. Read-only staff/protected reachability tests may use known-invalid sentinel IDs and assert documented negative `PAPIErrorCode` values. Record-set content add/remove/put tests are mutating and remain inconclusive placeholders until disposable record-set fixtures are configured and an executable cleanup strategy is implemented.

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
| Synch | synch bibs by ID (staff/protected-gated) | None | None | None |
| Patron reads | validate, basic data, blocks, hold/ILL/items out, messages, preferences, reading history, saved searches; barcode-from-id, renew blocks, and patron search are staff/protected-gated | None | None | None |
| Patron account/title lists | account get, title lists get | None by default for payment/refund/void/title-list mutations | focused title-list surface workflow creates generated lists, optionally exercises add/copy/move/delete content operations with `TestSettings:BibId`, and cleans up generated lists | payment/refund/void require disposable account fixtures; account-credit create/deposit remains a placeholder until generated artifacts can be uniquely identified and reversed |
| Hold requests/circulation | hold request list (staff/protected-gated) | None by default for hold/item mutations | None without disposable fixtures | hold create/cancel/reactivate/suspend/reply/pickup updates, item renew, renew-all, and item barcode update require disposable fixtures |
| Patron mutations/messages | None by default | None by default for patron message/reading-history/notification mutations | patron blocks, no-op patron update, username unauthorized check, notes update | patron message, reading-history, notification update, and registration v1/v2 require disposable fixtures |
| Staff/protected/record sets | hold request list, SA value lookup, notification queue when staff tests are enabled | read-only record-set get with a known-invalid sentinel ID | None without disposable fixtures | record-set add/remove/put mutations and remote storage require explicit scenario data |
