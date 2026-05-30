# 4.0.0-alpha.1 async migration

Polaris API 4.0.0-alpha.1 is an async-first breaking release. It keeps the existing request semantics, authentication behavior, and response models, but changes the public client surface so downstream callers can consistently await I/O and pass cancellation through every request path.

## Client method changes

All public `PapiClient`/`IPapiClient` methods are now `*Async` methods and return `Task<IRestResponse<T>>`.

```csharp
// 3.x
var response = client.BibSearch(options);

// 4.0
var response = await client.BibSearchAsync(options, cancellationToken);
```

Every public client method accepts `CancellationToken cancellationToken = default`, and the token is propagated through public, protected, and staff-authentication requests.

## Removed synchronous compatibility wrappers

The previous synchronous compatibility wrappers, including sync `Execute<T>`/`Post<T>` paths, were intentionally removed rather than retained as blocking shims. Consumers should migrate call sites to `await` the new async methods.

## Reading history overloads

`PatronReadingHistoryClear` overloads that previously accepted `params int[]` now accept `IEnumerable<int>`. This allows `CancellationToken` to remain the final optional parameter across the public API.
