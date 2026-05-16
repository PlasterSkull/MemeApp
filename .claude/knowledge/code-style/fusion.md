# Fusion Coding Patterns

See also: `.claude/knowledge/architecture/fusion.md` for architectural overview.

## Service Interface

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default);

    [ComputeMethod]
    Task<ImmutableList<MemeDto>> ListByTagAsync(string tag, CancellationToken ct = default);
}
```

## Service Implementation

```csharp
public class MemeService(IDbContextFactory<AppDbContext> dbFactory) : IMemeService
{
    public virtual async Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var meme = await db.Memes.FindAsync([id], ct);
        return meme?.ToDto();
    }

    public virtual async Task<ImmutableList<MemeDto>> ListByTagAsync(
        string tag, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return (await db.Memes
            .Where(m => m.Tags.Any(t => t.Slug == tag))
            .ToListAsync(ct))
            .Select(m => m.ToDto())
            .ToImmutableList();
    }
}
```

## Rules

- Computed methods must be `virtual` (Fusion proxies override them)
- Return type: `Task<T>` — not `ValueTask`, not `T`
- `CancellationToken ct = default` is always the last parameter
- Mutations call `Computed.Invalidate()` after persisting changes
- Never store mutable state in a Fusion service — all state goes to DB

## Invalidation Pattern

```csharp
public async Task DeleteAsync(Guid id, CancellationToken ct = default)
{
    await using var db = await dbFactory.CreateDbContextAsync(ct);
    await db.Memes.Where(m => m.Id == id).ExecuteDeleteAsync(ct);

    using (Computed.Invalidate())
    {
        _ = GetAsync(id);
    }
}
```

## Registration

```csharp
var fusion = services.AddFusion();
fusion.AddService<IMemeService, MemeService>();
```
