# 4.0 Async API Migration

Version 4.0 starts the async-first, async-only release line. The beta package is `4.0.0-beta.1`. It intentionally removes the synchronous compatibility wrappers from the 3.x client API.

## Public client methods are async-only

All public client methods now use the `*Async` suffix and return `Task<IRestResponse<T>>`.

```csharp
// 3.x
var response = client.BibSearch(options);

// 4.0
var response = await client.BibSearchAsync(options, cancellationToken);
```

### Before and after example

```csharp
// 3.x synchronous call
var response = client.PatronBasicDataGet(barcode, password);

if (response.Response.IsSuccessStatusCode)
{
    var patron = response.Data;
}
```

```csharp
// 4.0 async call
var response = await client.PatronBasicDataGetAsync(
    barcode,
    password,
    cancellationToken: cancellationToken);

if (response.Response.IsSuccessStatusCode)
{
    var patron = response.Data;
}
```

## CancellationToken support

Public client methods accept `CancellationToken cancellationToken = default` so callers can cancel both the final PAPI request and any required authentication request, such as protected-token acquisition.

## Target framework

`Clc.Polaris.Api` 4.0 targets `net8.0`. Consumers must run on .NET 8 or later.

## Staff override behavior

Staff override token acquisition is client-managed through `PapiClient.StaffOverrideAccount`. Staff override applies to protected methods and public patron-account methods that support staff override. General public methods are signed normally and should not receive staff override credentials.

## Protected token ownership

`PapiClient.Token` is readable for diagnostics and inspection. External callers should not assign protected tokens directly; configure `StaffOverrideAccount` and let the client acquire, cache, expire, and refresh protected tokens automatically.

## Removed sync wrappers

The former synchronous `Execute<T>` and `Post<T>` compatibility shims were intentionally removed. Callers should await the async methods instead of blocking on them.

## Patron reading history overloads

`PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>`. This keeps `CancellationToken` as the final optional parameter while still allowing callers to pass arrays or other integer sequences.

```csharp
IEnumerable<int> readingHistoryIds = new[] { 12345, 67890 };

var response = await client.PatronReadingHistoryClearAsync(
    barcode,
    readingHistoryIds,
    cancellationToken);
```

## Protected-token path requests

Some protected PAPI endpoints include the protected access token in the URL path. In 4.0, the built-in client methods for those endpoints use `ProtectedToken.Placeholder` internally for the protected-token URL path segment.

`ExecutePapiAsync` replaces the placeholder with the active protected access token before the request is sent and before PAPI hash formatting occurs. This allows the final URL and authorization hash to use the same tokenized path.

Consumers generally should not need to use `ProtectedToken.Placeholder` directly unless they are constructing a `PapiRestRequest` manually. Use `ExecutePapiAsync` to execute manual `PapiRestRequest` instances so placeholder replacement, token handling, and PAPI signing happen consistently.
