# AGENTS.md

## Scope

These instructions apply to the entire repository.

This repository is a C#/.NET library and test suite for the Polaris API client. Prefer small, behavior-preserving changes unless the task explicitly asks for a functional refactor.

## General expectations

- Preserve runtime behavior unless the task explicitly requests behavior changes.
- Prefer readable code over minimizing line count.
- Do not introduce new analyzer warnings, build warnings, or test failures.
- Do not add package references unless explicitly asked.
- Do not rename public APIs unless explicitly asked.
- Keep changes scoped to the task. Avoid drive-by refactors outside touched code.
- Before finishing, review the diff for accidental value changes, especially string literals used by assertions.


## Test organization

Tests use intent-first organization. Organize tests by the behavior they protect, not by mirroring production-code folders.

- Unit tests go in `tests/Clc.Polaris.Api.UnitTests`.
- Live integration tests go in `tests/Clc.Polaris.Api.LiveIntegrationTests`.
- Do not add tests to legacy `src/polaris-api-csharpTests`.
- Folder paths and namespaces carry behavior context.
- Filenames should be simple subject-based names: `<Subject>Tests.cs`.
- Do not use behavior-suffixed filenames such as:
  - `PatronSearch.Request.Tests.cs`
  - `PatronSearch.Cancellation.Tests.cs`
  - `PatronSearch.ProtectedToken.Tests.cs`
  - `BibSearch.Live.Tests.cs`
  - `BibGetByTypeV2ReadOnly.LiveTests.cs`
  - `JobsPurchaseOrdersPostResult.JsonDeserialization.Tests.cs`
- Do not use generic leaf filenames such as `Tests.cs`, `RequestTests.cs`, or `ModelTests.cs`.

### Unit test folders

- `PublicApiContracts/`: direct public API behavior, cancellation-token propagation, custom execution API contracts, and direct async/public API behavior.
- `EndpointRequests/`: endpoint-specific request construction such as route, HTTP method, query string, request body, content type, and endpoint-specific headers.
- `RequestPipeline/`: shared request preparation and mutation behavior such as authorization headers, signing, content handling, routing, staff override, known-token insertion, and idempotency.
- `AuthenticationAndTokens/`: staff authentication execution, protected-token acquisition, protected-token caching, protected-token expiration, token cache lock behavior, and token-specific model behavior.
- `ValidationContracts/`: guard clauses, argument validation, options validation, configuration validation, and invalid input behavior.
- `SerializationAndModels/`: JSON/XML serialization and deserialization, and general request/result model behavior that is not better classified elsewhere.
- `Governance/`: meta-tests that enforce test coverage, folder conventions, naming conventions, API-surface coverage, and category conventions.
- `Infrastructure/`: broadly reused test helpers only. Do not move endpoint-specific helpers into `Infrastructure/` unless they are reused across multiple test categories; keep one-off helpers private in the relevant test class.

### Unit placement precedence

When multiple folders seem plausible, use this order:

1. Invalid input or invalid configuration behavior -> `ValidationContracts/`.
2. Direct public API behavior, cancellation, or custom execution behavior -> `PublicApiContracts/`.
3. Endpoint-specific URL/body/query/header shape -> `EndpointRequests/`.
4. Shared request preformatting, signing, content, routing, token insertion, staff override, or idempotency -> `RequestPipeline/`.
5. Staff authentication execution, protected-token acquisition, token caching, token expiration, token locks, or token-specific model behavior -> `AuthenticationAndTokens/`.
6. JSON/XML serialization/deserialization or general request/result model behavior -> `SerializationAndModels/`.
7. Test-suite rule enforcement -> `Governance/`.

### Authentication and token boundaries

Keep authentication-related tests separated by what they prove:

- `EndpointRequests/Authentication/`: request shape for authentication endpoints, including route, HTTP method, request body, query string, and content type.
- `RequestPipeline/Authorization/`: signing, authorization headers, preformatting, and authorization request mutation.
- `RequestPipeline/Tokens/`: adding an already-known token to a request.
- `AuthenticationAndTokens/`: executing authentication, acquiring protected tokens, caching protected tokens, refreshing/expiring tokens, lock behavior, and token-specific model behavior.

Do not place protected-token acquisition/cache tests under `EndpointRequests/`. Do not place request-shape tests under `AuthenticationAndTokens/`.

`AuthenticationAndTokens/TokenModel/` is only for token-specific model behavior. General request/result model behavior belongs in `SerializationAndModels/ModelBehavior/`.

### Namespace rule

Namespaces should follow the test folder taxonomy. For example, `tests/Clc.Polaris.Api.UnitTests/EndpointRequests/Patron/PatronSearchTests.cs` should use `namespace Clc.Polaris.Api.UnitTests.EndpointRequests.Patron;`.

### Live integration tests

Live integration tests are organized by safety tier first, then domain. Use only these top-level folders, plus project files and global usings:

- `Configuration/`
- `Infrastructure/`
- `ReadOnly/`
- `ProtectedReadOnly/`
- `Mutating/`
- `ProtectedMutating/`
- `Scenarios/`

Use simple subject-based filenames: `<EndpointOrScenario>Tests.cs`. Do not include `.Live`, `ReadOnly`, `ProtectedReadOnly`, `Mutating`, or `ProtectedMutating` in the filename when the project/folder already supplies that classification.

MSTest categories must align with the live integration folder:

- `ReadOnly/` -> `TestCategory=ReadOnly`
- `ProtectedReadOnly/` -> `TestCategory=ProtectedReadOnly`
- `Mutating/` -> `TestCategory=Mutating`
- `ProtectedMutating/` -> `TestCategory=ProtectedMutating`
- `Scenarios/` -> `TestCategory=Lifecycle` or `TestCategory=ProtectedLifecycle`

### Reorganization-only constraints

For test organization changes:

- Move and rename files only.
- Update namespaces to match new folders.
- Do not rewrite assertions, test data, response strings, helpers, or production code.
- Do not add package references.
- Do not change public APIs.
- Do not introduce analyzer warnings, build warnings, or test failures.
- Run tests and review the diff for accidental assertion/string changes before finishing.

## C# formatting style

- Use ordinary C# formatting consistent with nearby files.
- Avoid unnecessary vertical wrapping.
- Keep simple constructor and method calls on one line when readable.
- Prefer local variables when an inline expression becomes visually dense.
- Keep one blank line between methods and test methods.
- Avoid double/triple blank-line gaps.
- Normalize using order in touched files:
  - `System.*`
  - `Clc.*`
  - `Microsoft.*`

## Test infrastructure

Prefer shared test infrastructure from `PapiClientTestBase` instead of adding new local helper classes.

Use the existing helpers where applicable:

```csharp
CreateJson(...)
CreatePapiResponseJson()
CreatePapiResponseJson(1)
CreateEmptyJsonObject()
CreateProtectedTokenJson(accessToken: "...", accessSecret: "...", expirationDate: ValidProtectedTokenExpirationDate)
ValidProtectedTokenExpirationDate
ExpiredProtectedTokenExpirationDate
```

### JSON response helpers

Do not hand-build escaped JSON strings such as:

```csharp
"{\"PAPIErrorCode\":0}"
"{\"PAPIErrorCode\":1}"
"{\"PAPIErrorCode\":0,\"AccessToken\":\"t\",\"AccessSecret\":\"s\",\"AuthExpDate\":\"2030-01-01T00:00:00Z\"}"
```

Use helpers instead:

```csharp
CreatePapiResponseJson()
CreatePapiResponseJson(1)
CreateProtectedTokenJson(accessToken: "t", accessSecret: "s", expirationDate: ValidProtectedTokenExpirationDate)
```

Use `CreateProtectedTokenJson(...)` only for protected-token/staff-authentication responses that contain:

```json
{
  "PAPIErrorCode": 0,
  "AccessToken": "...",
  "AccessSecret": "...",
  "AuthExpDate": "..."
}
```

Do not use `CreateProtectedTokenJson(...)` for patron-authentication responses or other response shapes. For other JSON response shapes, use `CreateJson(new { ... })` or add a clearly named helper if the shape is repeated.

Example patron authentication response:

```csharp
CreateJson(new { PAPIErrorCode = 0, AccessToken = "mock-token", AccessSecret = "mock-secret", PatronID = 123 })
```

Preserve intentional literal responses when the test specifically requires them, especially:

```csharp
"null"
```

### Protected-token dates

Use the shared date properties when the test only cares whether a token is valid or expired:

```csharp
ValidProtectedTokenExpirationDate
ExpiredProtectedTokenExpirationDate
```

Do not replace explicit `DateTime.UtcNow`, `DateTime.SpecifyKind`, or expiration-skew values when the test is specifically about:

- UTC expiration handling.
- Unspecified `DateTimeKind`.
- Expiration skew.
- Missing/null expiration dates.
- Exact timestamp behavior.

### Test HTTP handlers

Prefer `CapturingHttpMessageHandler` for tests that need to inspect the request or request body.

`CapturingHttpMessageHandler` captures:

```csharp
LastRequest
LastRequestContent
LastCancellationToken
RequestCount
```

and allows a custom JSON response:

```csharp
var handler = new CapturingHttpMessageHandler(CreatePapiResponseJson());
```

or:

```csharp
var handler = new CapturingHttpMessageHandler(CreateJson(new { PAPIErrorCode = 0, AccessToken = "mock-token", AccessSecret = "mock-secret", PatronID = 123 }));
```

Do not introduce local nested handlers with names that collide with base test infrastructure, such as:

```csharp
CaptureHttpMessageHandler
CapturingHttpMessageHandler
ProtectedTokenHttpMessageHandler
```

If a new handler is genuinely needed, give it a specific name that describes its behavior.

## MSTest expectations

### TestContext

If a shared test base class exposes `TestContext`, use this pattern:

```csharp
public TestContext TestContext { get; set; } = null!;
```

MSTest initializes this property at runtime. The `= null!;` initializer is intentional and avoids nullable initialization warnings.

If a test class inherits from a base class that already provides `TestContext`, do not redeclare it in the child class.

### Cancellation tokens

When calling async methods that expose a `CancellationToken` overload, pass the MSTest cancellation token:

```csharp
await client.AuthenticateStaffUserAsync(user, cancellationToken: TestContext.CancellationToken);
```

Do not call the overload without a cancellation token when a suitable overload exists:

```csharp
await client.AuthenticateStaffUserAsync(user);
```

### MSTEST0037: Use specific assertion methods

Prefer specific MSTest assertions over generic boolean assertions when available.

Use:

```csharp
Assert.IsNotEmpty(items);
Assert.Contains("expected", actual);
Assert.AreEqual(expected, actual);
Assert.IsNull(value);
Assert.IsNotNull(value);
```

instead of:

```csharp
Assert.IsTrue(items.Length > 0);
Assert.IsTrue(actual.Contains("expected"));
Assert.IsTrue(expected == actual);
Assert.IsTrue(value == null);
Assert.IsTrue(value != null);
```

Do not change assertion meaning just to satisfy the analyzer. If the specific assertion would obscure the intent or change comparison semantics, preserve the clearer assertion.

### MSTEST0046: Assert.Contains vs StringAssert.Contains

Prefer `Assert.Contains(...)` over `StringAssert.Contains(...)`.

Important: the argument order is different.

`StringAssert.Contains` uses:

```csharp
StringAssert.Contains(actualString, substring);
```

`Assert.Contains` uses:

```csharp
Assert.Contains(substring, value);
```

Correct conversion:

```csharp
StringAssert.Contains(handler.LastRequestContent, "secret");
Assert.Contains("secret", handler.LastRequestContent);
```

Incorrect conversion:

```csharp
Assert.Contains(handler.LastRequestContent, "secret");
```

Do not blindly trust IDE quick fixes for this rule. Verify the first argument is the expected substring and the second argument is the actual value being searched.

For URI assertions:

```csharp
Assert.Contains("/protected/v1/1033/100/1/authenticator/staff", handler.LastRequest!.RequestUri!.AbsolutePath);
```

not:

```csharp
Assert.Contains(handler.LastRequest!.RequestUri!.AbsolutePath, "/protected/v1/1033/100/1/authenticator/staff");
```

### Request body assertions

After asserting captured request content is not null:

```csharp
Assert.IsNotNull(handler.LastRequestContent);
```

use `Assert.Contains` with expected substring first:

```csharp
Assert.Contains("main", handler.LastRequestContent);
Assert.Contains("staff", handler.LastRequestContent);
Assert.Contains("secret", handler.LastRequestContent);
```

## Object construction and named arguments

Use named arguments when positional values are easy to confuse.

Preferred:

```csharp
new PolarisUser(domain: "main", username: "staff", password: "secret")
```

Acceptable when object initializer clarity is useful:

```csharp
new PolarisUser
{
    Domain = "main",
    Username = "staff",
    Password = "secret"
}
```

Be careful not to change assertion-sensitive literals during refactors. For example, do not accidentally change `"secret"` to `"sercret"`.

For protected token JSON helper calls, use named arguments:

```csharp
CreateProtectedTokenJson(accessToken: "t", accessSecret: "s", expirationDate: ValidProtectedTokenExpirationDate)
```

Do not use unclear positional calls:

```csharp
CreateProtectedTokenJson("t", "s", ValidProtectedTokenExpirationDate)
```

## Analyzer cleanup

Do not introduce new diagnostics for:

- `MSTEST0037`
- `MSTEST0046`
- `MSTEST0049`
- Nullable initialization warnings such as `CS8618`
- Member hiding warnings such as `CS0108`

If touching tests, fix analyzer diagnostics in the touched files unless the task explicitly says not to.

## Validation

Before finishing a change that touches source or tests, run the relevant commands from the repository root.

For library changes:

```bash
dotnet build src/polaris-api-csharp/Clc.Polaris.Api.csproj
```

For test changes:

```bash
dotnet build tests/Clc.Polaris.Api.UnitTests/Clc.Polaris.Api.UnitTests.csproj
dotnet test tests/Clc.Polaris.Api.UnitTests/Clc.Polaris.Api.UnitTests.csproj
```

If a command cannot be run in the environment, say so explicitly in the summary.

## Final response expectations

When summarizing work:

- List the files changed.
- Note any behavior changes, or explicitly state that behavior was preserved.
- Report build/test commands run and their results.
- Mention any remaining diagnostics or skipped validation.