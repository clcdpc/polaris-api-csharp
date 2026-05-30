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

## CancellationToken support

Public client methods accept `CancellationToken cancellationToken = default` so callers can cancel both the final PAPI request and any required authentication request, such as protected-token acquisition.

## Removed sync wrappers

The former synchronous `Execute<T>` and `Post<T>` compatibility shims were intentionally removed. Callers should await the async methods instead of blocking on them.

### Before and after

```csharp
// 3.x synchronous call
var response = client.PatronAccountGet("21221012345678", "patron-password");

// 4.0 async call
var response = await client.PatronAccountGetAsync(
    "21221012345678",
    "patron-password",
    cancellationToken);
```

## Protected-token path requests

Protected-token URL path segments are represented internally with `ProtectedToken.Placeholder`. `ExecutePapiAsync` replaces the placeholder with the current protected access token before the request is sent and before PAPI hash formatting occurs.

Most consumers do not need to use `ProtectedToken.Placeholder` directly. The main exception is advanced code that constructs a `PapiRestRequest` manually for an endpoint whose URL path contains the protected access token.

## Patron reading history overloads

`PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>`. This keeps `CancellationToken` as the final optional parameter while still allowing callers to pass arrays or other integer sequences.

```csharp
IEnumerable<int> readingHistoryIds = new[] { 1001, 1002, 1003 };

var response = await client.PatronReadingHistoryClearAsync(
    barcode: "21221012345678",
    password: "patron-password",
    ids: readingHistoryIds,
    cancellationToken: cancellationToken);
```
