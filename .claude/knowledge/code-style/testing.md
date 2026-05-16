# Testing

No test projects exist yet (except `MemeApp.Common.Tests`).
This file documents conventions to follow when tests are added.

## Project Structure

Each `Services` project gets a paired test project:

```
Core/MemeData/Services/          → Core/MemeData/Services.Tests/
Core/Users/Services/             → Core/Users/Services.Tests/
```

Tests for `MemeApp.Common` live in `Shared/MemeApp.Common.Tests/` (already exists).

## Naming

```
[MethodName]_[Scenario]_[ExpectedResult]

GetAsync_MemeExists_ReturnsDto
GetAsync_IdNotFound_ReturnsNull
AddTagAsync_DuplicateSlug_Throws
```

## Test Structure (AAA)

```csharp
[Fact]
public async Task GetAsync_MemeExists_ReturnsDto()
{
    // Arrange
    var id = Guid.NewGuid();
    // ... setup ...

    // Act
    var result = await _service.GetAsync(id);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(id, result.Id);
}
```

## What to Test

- Service logic: input validation, data mapping, edge cases
- Computed invalidation: after mutation, dependent compute result is invalidated
- Domain invariants: slug uniqueness, media type enum values

## What NOT to Test

- EF Core query syntax (test against real DB if needed, not mocked DbSet)
- Fusion infrastructure itself
- MudBlazor rendering

## Fusion Testing

Use `TestServices` from `ActualLab.Testing` to spin up an in-process Fusion host:

```csharp
var services = new ServiceCollection()
    .AddFusion()
    .AddService<IMemeService, MemeService>()
    .BuildServiceProvider();

var memeService = services.GetRequiredService<IMemeService>();
```
