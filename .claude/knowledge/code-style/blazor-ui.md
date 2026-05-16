# Blazor / UI Patterns

## Component Structure

Each component: one `.razor` file + optional `ComponentName.razor.cs` code-behind.
Keep markup in `.razor`, logic in code-behind.

```
MemeCard.razor          ← markup
MemeCard.razor.cs       ← @code logic (partial class)
```

## MudBlazor

Use MudBlazor components over raw HTML. Key components:
- `MudCard`, `MudCardMedia` — meme display cards
- `MudChip`, `MudChipSet` — tag display
- `MudTextField`, `MudSelect` — forms
- `MudGrid`, `MudItem` — layout

## Fusion Live Data in Blazor

Inherit from `ComputedStateComponent<T>` for reactive data binding:

```csharp
@inherits ComputedStateComponent<ImmutableList<MemeDto>>

protected override async Task<ImmutableList<MemeDto>> ComputeState(CancellationToken ct)
    => await MemeService.ListByTagAsync(CurrentTag, ct);
```

The component re-renders automatically when the computed value is invalidated.

## Rules

- No direct HTTP calls from components — always through Fusion services
- No `StateHasChanged()` calls for Fusion-reactive state — it updates automatically
- `@inject` preferred over constructor injection in `.razor` files
- Avoid `OnInitializedAsync` for data that should be reactive — use `ComputedStateComponent`
