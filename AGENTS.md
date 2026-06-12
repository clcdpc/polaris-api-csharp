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
dotnet build src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj
dotnet test src/polaris-api-csharpTests/Clc.Polaris.Api.Tests.csproj --filter "TestCategory=Unit"
```

If a command cannot be run in the environment, say so explicitly in the summary.

## Final response expectations

When summarizing work:

- List the files changed.
- Note any behavior changes, or explicitly state that behavior was preserved.
- Report build/test commands run and their results.
- Mention any remaining diagnostics or skipped validation.