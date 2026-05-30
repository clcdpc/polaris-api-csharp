# 4.0 async migration notes

Version `4.0.0-alpha.1` is an async-first, async-only API release. This note covers the breaking client API changes consumers need when moving from 3.x.

## Public client methods are async-only

All public `PapiClient` methods now use the `*Async` suffix and return `Task<IRestResponse<T>>`. Pass a `CancellationToken` when the caller should be able to cancel in-flight PAPI requests.

```csharp
// 3.x
var response = client.BibSearch(options);

// 4.0.0-alpha.1
var response = await client.BibSearchAsync(options, cancellationToken);
```

Synchronous compatibility wrappers such as `Execute<T>` and `Post<T>` were intentionally removed rather than retained as sync-over-async shims.

## CancellationToken placement

`CancellationToken cancellationToken = default` is accepted across the public API and remains the final optional parameter. To preserve that convention, `PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>` instead.
