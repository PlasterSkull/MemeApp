# Formatting

Source of truth: `.editorconfig` at repo root.

## Braces — Allman style

Opening brace on new line for: types, methods, control blocks, properties, accessors, lambdas.

```csharp
public class MemeService
{
    public async Task<MemeDto?> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return null;
        }
        return await _repo.FindAsync(id);
    }
}
```

No braces preferred for single-statement blocks (`csharp_prefer_braces = false`):

```csharp
if (id == Guid.Empty)
    return null;
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

// Constructor — block body
public MemeService(IRepository repo)
{
    _repo = repo;
}
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
var results = memes.Where(m => ...).ToList();  // complex
```

## Indentation & Line Endings

- 4 spaces (no tabs) for `.cs` files
- CRLF line endings
- 2 spaces for `.json`, `.xml`, `.csproj`, `.props`

## Modifiers Order

```
public private internal protected async static readonly sealed override abstract
```

## Other

- No `this.` qualification on any member
- No primary constructors (`csharp_style_prefer_primary_constructors = false`)
- File-scoped namespaces (see file-namespace.md)
- `using` directives outside namespace
