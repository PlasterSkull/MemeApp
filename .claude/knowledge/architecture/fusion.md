# ActualLab.Fusion Architecture

## What Fusion Does

Fusion provides transparent reactive caching.
A `[ComputeMethod]` result is cached and automatically invalidated
when its dependencies change — clients get live updates without polling.

## IComputeService

All Fusion-reactive services implement `IComputeService`:

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default);

    [ComputeMethod]
    Task<ImmutableList<MemeDto>> ListByTagAsync(string tag, CancellationToken ct = default);
}
```

Rules:
- Interface must extend `IComputeService`
- Reactive methods get `[ComputeMethod]` attribute
- Return type must be `Task<T>` (not `ValueTask`)
- Last parameter is always `CancellationToken ct = default`

## Invalidation

Mutations invalidate affected compute results:

```csharp
public async Task AddTagAsync(Guid memeId, string tag, CancellationToken ct = default)
{
    // ... persist ...
    using (Computed.Invalidate())
    {
        _ = GetAsync(memeId);
        _ = ListByTagAsync(tag);
    }
}
```

## Blazor Integration

Use `ComputedStateComponent<T>` as base class for reactive components:

```csharp
@inherits ComputedStateComponent<ImmutableList<MemeDto>>

protected override async Task<ImmutableList<MemeDto>> ComputeState(CancellationToken ct)
    => await MemeService.ListByTagAsync(CurrentTag, ct);
```

The component re-renders automatically when the computed value is invalidated.

## Session / Auth

- `IAuth` provides session-based authentication
- `Session` flows through service method parameters
- Never store session in a field — always pass through the call chain

## Registration

```csharp
var fusion = services.AddFusion();
fusion.AddService<IMemeService, MemeService>();
```
