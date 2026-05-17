# Domain Model

## Strongly Typed IDs

All domain entities use strongly typed IDs via `StronglyTypedId` (not plain `Guid`).
This prevents mixing up IDs across different entity types at compile time.

```csharp
[StronglyTypedId]
public partial struct MemeId { }

[StronglyTypedId]
public partial struct TagId { }

[StronglyTypedId]
public partial struct CollectionId { }
```

**Where to use:** Domain entity classes.
**Where NOT to use:** EF Core DB model classes (use `Guid` there for ORM compatibility).

## Core Entities

### Meme
The central entity. Represents a single piece of media content.

| Property | Type | Notes |
|----------|------|-------|
| Id | MemeId | Strongly typed |
| Title | string | Optional display name |
| MediaType | MediaType | Photo / Video / Gif |
| SourceUrl | string? | Original URL if linked |
| FilePath | string? | Local path if uploaded |
| Tags | ICollection\<Tag\> | Many-to-many |
| CreatedAt | DateTime | UTC |

### MediaType

```csharp
public enum MediaType { Photo, Video, Gif }
```

### Tag
Flat label for categorization. No hierarchy.

| Property | Type | Notes |
|----------|------|-------|
| Id | TagId | Strongly typed |
| Name | string | Display name |
| Slug | string | URL-safe, lowercase, unique |

### Collection
Named group of memes. One meme can appear in many collections.

| Property | Type | Notes |
|----------|------|-------|
| Id | CollectionId | Strongly typed |
| Name | string | |
| Description | string? | |
| Memes | ICollection\<Meme\> | Many-to-many |

## Relationships

```
Meme ──< MemeTag >── Tag
Meme ──< CollectionMeme >── Collection
```
