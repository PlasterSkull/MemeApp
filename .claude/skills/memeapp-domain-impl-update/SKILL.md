---
name: memeapp-domain-impl-update
description: Use when applying an approved domain spec delta to existing MemeApp domain code — adds aggregates, operations, or events to already-existing projects without recreating project structure.
---

# MemeApp Domain Implementation Update

## Overview

Given an approved spec delta, apply changes to existing `src/dotnet/Domains/<Name>/` code.

**Prerequisite:** approved spec delta from `memeapp-domain-spec-update`. Do NOT start without one.

No new .csproj, no new slnx project entries, no GlobalUsings.cs — project scaffolding already exists.

## Change Types

### New Aggregate

Create in both Contracts and Core (same conventions as `memeapp-domain-impl`):

**Contracts — new folder `<NewAggregates>/`:**
```
<NewAggregates>/
  Models/
    <NewAggregate>.cs
    <NewAggregate>Id.cs
  Services/
    I<NewAggregate>BackendService.cs
    Commands/
      Add<NewAggregate>Command.cs
      ...
    Events/        (optional)
```

**Core — new folder `<NewAggregates>/`:**
```
<NewAggregates>/
  <NewAggregate>BackendService.cs
  <NewAggregate>Module.cs        ← [DependsOn<DatabaseModule>]
```

**Core — also add to DB layer:**
- `Database/Projections/Db<NewAggregate>.cs`
- Register DbSet in `<Name>DbContext.cs`

**Core — update root module:**
```csharp
// <Name>Module.cs — add DependsOn
[DependsOn<<ExistingAggregate>Module>]
[DependsOn<<NewAggregate>Module>]      // ← add this
public class <Name>Module : AppModule { }
```

All new files follow existing namespace (`MemeApp.<Name>.Contracts` / `MemeApp.<Name>.Core`), no `namespace` changes in existing files.

---

### New Operation on Existing Aggregate

**1. Add to interface** (`Contracts/<Aggregates>/Services/I<Aggregate>BackendService.cs`):

```csharp
// Query:
[ComputeMethod]
Task<ImmutableList<<Aggregate>>> ListBy<Filter>Async(<FilterType> filter, CancellationToken cancellationToken);

// Command:
[CommandHandler]
Task<<Aggregate>> <Verb>Async(<Verb><Aggregate>Command command, CancellationToken cancellationToken);
```

**2. Add command record** if new command (`Contracts/<Aggregates>/Services/Commands/`):
```csharp
public record <Verb><Aggregate>Command(...) : ICommand<<Aggregate>>;
```

**3. Add stub implementation** (`Core/<Aggregates>/<Aggregate>BackendService.cs`):

For queries:
```csharp
public virtual async Task<ImmutableList<<Aggregate>>> ListBy<Filter>Async(<FilterType> filter, CancellationToken cancellationToken)
{
    await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
    // TODO: implement
    throw new NotImplementedException();
}
```

For commands — include `Invalidation.Begin()`:
```csharp
public virtual async Task<<Aggregate>> <Verb>Async(<Verb><Aggregate>Command command, CancellationToken cancellationToken)
{
    await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
    // TODO: implement

    using (Invalidation.Begin())
        _ = ListAsync(default);

    throw new NotImplementedException();
}
```

---

### New or Modified Field on Existing Aggregate

**1. Update aggregate model** (`Contracts/<Aggregates>/Models/<Aggregate>.cs`):
- Add/remove/rename constructor parameter in the record
- Update related command records if the field is set via command

**2. Update DB projection** (`Core/Database/Projections/Db<Aggregate>.cs`):
- Add/remove/rename the corresponding CLR property

**3. Update DbContext configuration** if the field affects column mapping, constraints, or indexes.

**4. Update mapping** (`ToModel()` extension or inline projection) to reflect the new field.

**5. Check invalidation** — if the new field is filter-relevant, existing `[ComputeMethod]` queries may need broader invalidation.

> After field changes, an EF migration will be needed. Remind the user if schema-affecting fields were modified.

---

### New Event

<!-- TODO: описать паттерн EventCommand — EnqueueAfterCompletion, INotifyCommand, обработчики -->

---

### Modified Operation

- **Rename**: update interface signature + implementation method signature + all callers within the domain
- **Change return type**: update interface + implementation + command record if needed
- **Add/remove parameter**: update interface + implementation + command record

---

### Removed Operation

- Remove from interface
- Remove implementation method
- Remove command record file if no longer needed
- Remove event file if no longer needed

## Checklist

- [ ] New aggregate: plural folder name (`<NewAggregates>/`) in both Contracts and Core
- [ ] New aggregate: namespace `MemeApp.<Name>.Contracts` / `.Core` — no changes to existing files' namespaces
- [ ] New aggregate: `Db<NewAggregate>.cs` in Projections + DbSet registered in DbContext
- [ ] New aggregate: `<NewAggregate>Module` with `[DependsOn<DatabaseModule>]`
- [ ] New aggregate: root `<Name>Module` updated with new `[DependsOn]`
- [ ] New operation: `[ComputeMethod]` or `[CommandHandler]` on interface method
- [ ] New command operation: implementation method is `virtual` + `Invalidation.Begin()`
- [ ] New query operation: implementation method is `virtual`
- [ ] New event: `EventCommand, INotifyCommand` + `EnqueueAfterCompletion()` in handler
- [ ] No new .csproj, no new slnx project entries
- [ ] No changes to existing namespaces or GlobalUsings.cs
