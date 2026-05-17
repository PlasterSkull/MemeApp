# ActualLab.Fusion Architecture

## What Fusion Does

Fusion provides transparent reactive caching.
A `[ComputeMethod]` result is cached and automatically invalidated
when its dependencies change — clients get live updates without polling.

Cache key = `(service, method, arguments)`. When a value is invalidated,
all computed values that called it as a dependency are also invalidated (cascading DAG).

## Core Pattern

**Compute services** expose reactive methods marked `[ComputeMethod]`.
**Mutations** call `Invalidation.Begin()` to mark stale entries.
**Blazor components** inherit `ComputedStateComponent<T>` and re-render automatically.

## Invalidation

```csharp
using (Invalidation.Begin())
{
    _ = GetAsync(id);           // marks this call's cache entry stale
    _ = ListByTagAsync(tag);    // cascades to all dependents
}
```

Calls inside `Invalidation.Begin()` complete synchronously without executing the method body.
They only mark the matching cache entries as stale.

## Session / Auth

- `Session` is passed as a method parameter (never stored in a field)
- Services that scope results to a user take `Session session` as first parameter
- `IAuth.GetUser(session, ct)` creates a dependency on auth state — auto-invalidates on sign-out
- `ClientAuthHelper` and `ISessionResolver` are the entry points in Blazor components

For coding patterns (interfaces, components, invalidation examples) see:
`.claude/knowledge/code-style/fusion.md`
