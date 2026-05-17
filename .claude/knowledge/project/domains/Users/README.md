# Domain Spec: Users

## Aggregates

- `User` — учётная запись пользователя (профиль, идентификация)
- `UserSettings` — настройки пользователя 1:1 к User

## User

### Fields

| Field | Type | Notes |
|-------|------|-------|
| Id | `UserId` | Strongly typed, wraps `long` |
| Username | `string` | Уникальный |
| Email | `string` | Уникальный |
| AvatarUrl | `string?` | |
| CreatedAt | `DateTimeOffset` | |

### Queries

- `GetAsync(UserId)` → `User?`

### Commands

- `UpdateProfileAsync(UpdateUserProfileCommand)` → `User`
- `DeleteAsync(DeleteUserCommand)` → (none)

## UserSettings

### Fields

| Field | Type | Notes |
|-------|------|-------|
| UserId | `UserId` | PK + FK, wraps `long` |
| Theme | `Theme` (enum) | Light / Dark / System |

### Queries

- `GetAsync(UserId)` → `UserSettings?`

### Commands

- `UpdateAsync(UpdateUserSettingsCommand)` → `UserSettings`
