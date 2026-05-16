# Domain Model

## Core Entities

### Meme
The central entity. Represents a single piece of media content.

| Property | Type | Notes |
|----------|------|-------|
| Id | Guid | |
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
| Id | Guid | |
| Name | string | Display name |
| Slug | string | URL-safe, lowercase, unique |

### Collection
Named group of memes. One meme can appear in many collections.

| Property | Type | Notes |
|----------|------|-------|
| Id | Guid | |
| Name | string | |
| Description | string? | |
| Memes | ICollection\<Meme\> | Many-to-many |

## Relationships

```
Meme ──< MemeTag >── Tag
Meme ──< CollectionMeme >── Collection
```
