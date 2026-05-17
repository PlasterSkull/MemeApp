# Fusion Coding Patterns

See also: `.claude/knowledge/architecture/fusion.md` for conceptual overview.

---

## Service Interface

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<Meme?> GetAsync(MemeId id, CancellationToken cancellationToken);

    [ComputeMethod]
    Task<ImmutableList<Meme>> ListByTagAsync(string tag, CancellationToken cancellationToken);
}
```

Rules:
- Extend `IComputeService`
- `[ComputeMethod]` on every reactive method
- Return `Task<T>` — never `ValueTask`, never plain `T`
- `CancellationToken cancellationToken` always last — **no `= default`**, callers must pass it explicitly

---

## Service Implementation

```csharp
public class MemeService(IDbContextFactory<AppDbContext> dbFactory) : IMemeService
{
    public virtual async Task<Meme?> GetAsync(MemeId id, CancellationToken cancellationToken)
    {
        await using var dbContext = await dbFactory.CreateDbContextAsync(cancellationToken);
        var meme = await dbContext.Memes.FindAsync([id.Value], cancellationToken);
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
public async Task DeleteAsync(MemeId id, CancellationToken cancellationToken)
{
    await using var dbContext = await dbFactory.CreateDbContextAsync(cancellationToken);
    await dbContext.Memes.Where(meme => meme.Id == id.Value).ExecuteDeleteAsync(cancellationToken);

    using (Invalidation.Begin())
    {
        _ = GetAsync(id, default);
        _ = ListByTagAsync(default!, default);  // invalidate all tag lists
    }
}
```

Rules:
- `Invalidation.Begin()` — not `Computed.Invalidate()`
- Inside `Invalidation.Begin()` pass `default` for `cancellationToken` — calls are synchronous no-ops
- Calls inside `Invalidation.Begin()` only mark cache entries stale, method body does not execute

---

## Service Registration

```csharp
var fusion = services.AddFusion();
fusion.AddService<IMemeService, MemeService>();
```

---

## Session / Auth

When a method's result depends on who is calling, pass `Session` as first parameter:

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<ImmutableList<Meme>> GetMyMemesAsync(
        Session session,
        CancellationToken cancellationToken);
}

public class MemeService : IMemeService
{
    private readonly IAuth _auth;

    public virtual async Task<ImmutableList<Meme>> GetMyMemesAsync(
        Session session,
        CancellationToken cancellationToken)
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
