# Async Patterns

## Method Signatures

`CancellationToken cancellationToken` always last.

**Fusion services** — no `= default`. Callers must pass explicitly:
```csharp
Task<MemeDto?> GetAsync(MemeId id, CancellationToken cancellationToken);
Task<ImmutableList<MemeDto>> ListByTagAsync(string tag, CancellationToken cancellationToken);
```

**Non-Fusion methods** — `= default` allowed:
```csharp
Task AddTagAsync(MemeId memeId, string tagSlug, CancellationToken cancellationToken = default);
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
var memeTask = memeService.GetAsync(id, cancellationToken);
var tagsTask = tagService.ListForMemeAsync(id, cancellationToken);
await Task.WhenAll(memeTask, tagsTask);
var meme = memeTask.Result;
var tags = tagsTask.Result;
```
