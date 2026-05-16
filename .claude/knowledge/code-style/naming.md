# Naming Conventions

## Types

| Kind | Convention | Example |
|------|-----------|---------|
| Class | PascalCase | `MemeService`, `TagRepository` |
| Interface | `I` + PascalCase | `IMemeService`, `ITagRepository` |
| Enum | PascalCase | `MediaType` |
| Enum member | PascalCase | `MediaType.Photo` |
| Record / Struct | PascalCase | `MemeDto`, `TagSlug` |

## Members

| Kind | Convention | Example |
|------|-----------|---------|
| Public method | PascalCase | `GetAsync`, `AddTagAsync` |
| Public property | PascalCase | `CreatedAt`, `MediaType` |
| Private field | `_camelCase` | `_repository`, `_logger` |
| Local variable | camelCase | `meme`, `tagSlug` |
| Parameter | camelCase | `memeId`, `cancellationToken` |
| Const | PascalCase | `MaxTagLength` |

## DTOs

Suffix `Dto` on data transfer objects:

```csharp
public record MemeDto(Guid Id, string Title, MediaType MediaType);
public record TagDto(Guid Id, string Name, string Slug);
```

## Services and Interfaces

Interface in `Contracts/`, implementation in `Services/`.
Names match exactly minus the `I` prefix:

```
IMemeService  →  MemeService
ITagService   →  TagService
```

## Async Methods

All async methods end in `Async`:

```csharp
Task<MemeDto?> GetAsync(...)
Task AddTagAsync(...)
Task<ImmutableList<MemeDto>> ListByTagAsync(...)
```

## Lambda Variables (LINQ)

Always use full descriptive names in lambda expressions — no single letters or abbreviations:

```csharp
// Correct
memes.Where(meme => meme.MediaType == MediaType.Gif)
tags.Select(tag => tag.Slug)
collections.OrderBy(collection => collection.Name)

// Wrong
memes.Where(m => m.MediaType == MediaType.Gif)
tags.Select(t => t.Slug)
```

## No Abbreviations

Full words always. No exceptions for parameters or locals.
