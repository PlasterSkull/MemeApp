# Async Patterns

## Method Signatures

All service methods are async. `CancellationToken ct = default` is always last:

```csharp
Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default);
Task AddTagAsync(Guid memeId, string tagSlug, CancellationToken ct = default);
```

## Task vs ValueTask

- Use `Task<T>` for Fusion compute methods (required by Fusion)
- Use `ValueTask<T>` for hot paths that frequently return synchronously (e.g., cache hits)
- Default to `Task<T>` unless profiling shows ValueTask is worth it

## Never Block

Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` on tasks.

```csharp
// Wrong
var meme = _service.GetAsync(id).Result;

// Correct
var meme = await _service.GetAsync(id);
```

## ConfigureAwait

No `ConfigureAwait(false)` in application code (only in library code).
This project is application-layer, not a library.

## Parallel Work

Use `Task.WhenAll` for independent concurrent operations:

```csharp
var memeTask = memeService.GetAsync(id, ct);
var tagsTask = tagService.ListForMemeAsync(id, ct);
await Task.WhenAll(memeTask, tagsTask);
var meme = memeTask.Result;
var tags = tagsTask.Result;
```
