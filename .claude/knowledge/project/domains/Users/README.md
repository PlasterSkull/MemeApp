# Domain Spec: Users

## Aggregates

- `MemeUser` — учётная запись пользователя (профиль, идентификация)
- `MemeUserSettings` — настройки пользователя 1:1 к MemeUser

## MemeUser

### Fields

| Field | Type | Notes |
|-------|------|-------|
| Id | `MemeUserId` | Strongly typed, wraps `long` |
| Username | `string` | Уникальный |
| Email | `string` | Уникальный |
| AvatarUrl | `string?` | |
| CreatedAt | `DateTimeOffset` | |

### Queries

- `GetAsync(MemeUserId)` → `MemeUser?`

### Commands

- `UpdateProfileAsync(UpdateMemeUserProfileCommand)` → `MemeUser`
- `DeleteAsync(DeleteMemeUserCommand)` → (none)

## MemeUserSettings

### Fields

| Field | Type | Notes |
|-------|------|-------|
| MemeUserId | `MemeUserId` | PK + FK, wraps `long` |
| Theme | `Theme` (enum) | Light / Dark / System |

### Queries

- `GetAsync(MemeUserId)` → `MemeUserSettings?`

### Commands

- `UpdateAsync(UpdateMemeUserSettingsCommand)` → `MemeUserSettings`
