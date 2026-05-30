# 4.0 Async API Migration

Version `4.0.0-alpha.1` is an async-first, async-only release. It intentionally removes the synchronous compatibility wrappers from the 3.x client API.

## Public client methods are async-only

All public client methods now use the `*Async` suffix and return `Task<IRestResponse<T>>`.

```csharp
// 3.x
var response = client.BibSearch(options);

// 4.0
var response = await client.BibSearchAsync(options, cancellationToken);
```

### Before and after

A synchronous 3.x call should become an awaited 4.0 async call:

```csharp
// 3.x
var account = client.PatronAccountGet("21221012345678", "patron-password");

if (account.Data.PAPIErrorCode == 0)
{
    Console.WriteLine(account.Data.PatronAccountGetRows.Count);
}
```

```csharp
// 4.0
var account = await client.PatronAccountGetAsync(
    "21221012345678",
    "patron-password",
    cancellationToken);

if (account.Data.PAPIErrorCode == 0)
{
    Console.WriteLine(account.Data.PatronAccountGetRows.Count);
}
```

## CancellationToken support

Public client methods accept `CancellationToken cancellationToken = default` so callers can cancel both the final PAPI request and any required authentication request, such as protected-token acquisition.

## Removed sync wrappers

The former synchronous `Execute<T>` and `Post<T>` compatibility shims were intentionally removed. Callers should await the async methods instead of blocking on them.

## Protected-token path requests

Some protected PAPI endpoints require the protected access token as a URL path segment. Public client methods for those endpoints use `ProtectedToken.Placeholder` internally when constructing the request path.

`ExecutePapiAsync` replaces that placeholder with the current protected access token before the request is sent and before the PAPI hash is formatted. If `StaffOverrideAccount` is configured and a valid token is not already available, the client can acquire a protected token automatically before replacing the placeholder.

Consumers generally should not need to use `ProtectedToken.Placeholder` directly unless constructing a `PapiRestRequest` manually.

## Patron reading history overloads

`PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>`. This keeps `CancellationToken` as the final optional parameter while still allowing callers to pass arrays or other integer sequences.

```csharp
var readingHistoryIds = new[] { 101, 102, 103 };

var clearResponse = await client.PatronReadingHistoryClearAsync(
    "21221012345678",
    readingHistoryIds,
    cancellationToken);
```
