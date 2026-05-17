# Formatting

Source of truth: `.editorconfig` at repo root.

## Braces — Allman style

Opening brace on new line for: types, methods, control blocks, properties, accessors.

**Always use braces** for `if`/`else`/`for`/`foreach`/`while`/`using` blocks:

```csharp
if (id == Guid.Empty)
{
    return null;
}

foreach (var tag in tags)
{
    meme.Tags.Add(tag);
}
```

**Exception — LINQ lambda chains:** no braces, expression form only:

```csharp
var slugs = tags
    .Where(tag => tag.IsActive)
    .Select(tag => tag.Slug)
    .ToList();
```

## Primary Constructors

Prefer primary constructors when the constructor has no logic — only field assignment:

```csharp
// Preferred
public class MemeService(IDbContextFactory<AppDbContext> dbFactory) : IMemeService
{
    // ...
}

// Only when constructor logic is needed
public class MemeService : IMemeService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public MemeService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
    }
}
```

## Expression Bodies

| Member kind | Style |
|-------------|-------|
| Methods | Expression-bodied preferred |
| Properties | Expression-bodied preferred |
| Constructors | Block body required |
| Accessors (`get`/`set`) | Block body required |

```csharp
// Method — expression body
public string GetSlug() => Name.ToLowerInvariant().Replace(' ', '-');

// Property — expression body
public string DisplayName => $"{Name} ({MediaType})";
```

## var vs Explicit Type

| Case | Rule |
|------|------|
| Type apparent from RHS | `var` |
| Built-in types (`int`, `string`, `bool`, etc.) | Explicit |
| Complex generic or LINQ result | `var` |

```csharp
var meme = await _repo.FindAsync(id);          // type apparent
int count = memes.Count;                        // built-in
string name = tag.Name;                         // built-in
var results = memes.Where(tag => ...).ToList(); // complex
```

## Indentation & Line Endings

- 4 spaces (no tabs) for `.cs` files
- CRLF line endings
- 2 spaces for `.json`, `.xml`, `.csproj`, `.props`

## Modifiers Order

```
public private internal protected async static readonly sealed override abstract
```

## Immutable Collections

Prefer immutable collections for return types and domain model properties:

| Instead of | Use |
|-----------|-----|
| `List<T>` | `ImmutableList<T>` |
| `Dictionary<K,V>` | `ImmutableDictionary<K,V>` |
| `HashSet<T>` | `ImmutableHashSet<T>` |

```csharp
// Service return types
Task<ImmutableList<Meme>> ListByTagAsync(string tag, CancellationToken cancellationToken);

// Domain model properties
public ImmutableList<Tag> Tags { get; init; } = [];
```

Use mutable collections only inside method bodies when building up a result, then convert:

```csharp
var results = new List<Meme>();
// ... build ...
return results.ToImmutableList();
```

## Other

- No `this.` qualification on any member
- File-scoped namespaces (see file-namespace.md)
- `using` directives outside namespace
