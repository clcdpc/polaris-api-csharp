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

## Patron reading history overloads

`PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>`. This keeps `CancellationToken` as the final optional parameter while still allowing callers to pass arrays or other integer sequences.

## Protected-token path requests

Some protected PAPI endpoints include the protected access token as a URL path segment. In 4.0, built-in client methods for those endpoints use `ProtectedToken.Placeholder` internally for that path segment.

`ExecutePapiAsync` replaces `ProtectedToken.Placeholder` with the current protected access token before the request is sent and before the URL is formatted for the PAPI hash. Consumers generally should not need to use the placeholder directly unless constructing a `PapiRestRequest` manually.

## PatronReadingHistoryClearAsync example

`PatronReadingHistoryClearAsync` accepts an `IEnumerable<int>` for reading-history IDs:

```csharp
var idsToClear = new[] { 101, 102, 103 };

var response = await client.PatronReadingHistoryClearAsync(
    barcode: "21221000000000",
    ids: idsToClear,
    cancellationToken: cancellationToken);
```

## Before/after sync-to-async example

```csharp
// 3.x synchronous call
var syncResponse = client.PatronReadingHistoryClear("21221000000000", 101, 102, 103);

// 4.0 async call
var asyncResponse = await client.PatronReadingHistoryClearAsync(
    "21221000000000",
    new[] { 101, 102, 103 },
    cancellationToken);
```
