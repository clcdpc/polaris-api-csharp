# 4.0 async migration

`Clc.Polaris.Api` 4.0.0-alpha.1 is an async-first breaking API update. This note is intentionally limited to the public client API changes consumers need when moving from 3.x.

## Public client methods are async-only

All public `PapiClient`/`IPapiClient` operations now use `*Async` method names and return `Task<IRestResponse<T>>`.

```csharp
// 3.x
var response = client.BibSearch(options);

// 4.0.0-alpha.1
var response = await client.BibSearchAsync(options, cancellationToken);
```

A `CancellationToken` is accepted across the public API and is the final optional parameter where applicable. Synchronous compatibility wrappers such as `Execute<T>`/`Post<T>` and the old non-async public client methods were intentionally removed rather than retained as blocking shims.

## Reading history clear overloads

`PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>` so `CancellationToken` can remain the final optional parameter.

```csharp
// 3.x
client.PatronReadingHistoryClear(barcode, 101, 102, 103);

// 4.0.0-alpha.1
await client.PatronReadingHistoryClearAsync(barcode, new[] { 101, 102, 103 }, cancellationToken);
```
