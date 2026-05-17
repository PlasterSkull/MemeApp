# Fusion Coding Patterns

See also: `.claude/knowledge/architecture/fusion.md` for conceptual overview.

---

## Service Interface

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<MemeDto?> GetAsync(MemeId id, CancellationToken cancellationToken = default);

    [ComputeMethod]
    Task<ImmutableList<MemeDto>> ListByTagAsync(string tag, CancellationToken cancellationToken = default);
}
```

Rules:
- Extend `IComputeService`
- `[ComputeMethod]` on every reactive method
- Return `Task<T>` — never `ValueTask`, never plain `T`
- `CancellationToken cancellationToken = default` always last

---

## Service Implementation

```csharp
public class MemeService(IDbContextFactory<AppDbContext> dbFactory) : IMemeService
{
    public virtual async Task<MemeDto?> GetAsync(MemeId id, CancellationToken cancellationToken = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var meme = await db.Memes.FindAsync([id.Value], cancellationToken);
        return meme?.ToDto();
    }
}
```

Rules:
- Computed methods must be `virtual` (Fusion proxy overrides them)
- Primary constructor preferred when no constructor logic

---

## Invalidation

```csharp
public async Task DeleteAsync(MemeId id, CancellationToken cancellationToken = default)
{
    await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
    await db.Memes.Where(meme => meme.Id == id.Value).ExecuteDeleteAsync(cancellationToken);

    using (Invalidation.Begin())
    {
        _ = GetAsync(id);
        _ = ListByTagAsync(default!);  // invalidate all tag lists
    }
}
```

Rules:
- `Invalidation.Begin()` — not `Computed.Invalidate()`
- Call the exact method signatures whose cache should be dropped
- Calls inside `Invalidation.Begin()` are synchronous no-ops that mark cache entries stale

---

## Service Registration

```csharp
var fusion = services.AddFusion();
fusion.AddService<IMemeService, MemeService>();
```

For Blazor host add:
```csharp
services.AddFusion().AddBlazor();
```

---

## Session / Auth

When a method's result depends on who is calling, pass `Session` as first parameter:

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<ImmutableList<MemeDto>> GetMyMemesAsync(
        Session session,
        CancellationToken cancellationToken = default);
}

public class MemeService : IMemeService
{
    private readonly IAuth _auth;

    public virtual async Task<ImmutableList<MemeDto>> GetMyMemesAsync(
        Session session,
        CancellationToken cancellationToken = default)
    {
        var user = await _auth.GetUser(session, cancellationToken).Require();
        // result auto-invalidates when user signs out
        return await GetForUserAsync(user.Id, cancellationToken);
    }
}
```

Rules:
- `Session` before all other params, before `CancellationToken`
- Never store `Session` in a field — always pass through call chain
- `.Require()` throws if not authenticated; `.Require(User.MustBeAuthenticated)` also checks auth status

---

## Blazor Component (state)

> TODO: Fill in when Blazor UI work begins. See `blazor-ui.md`.
