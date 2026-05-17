---
name: memeapp-domain-impl
description: Use when implementing a MemeApp domain from an approved domain spec — creates project structure, csproj files, interfaces with CommandHandler, event commands, DB projections, and DI modules following MemeApp conventions.
---

# MemeApp Domain Implementation

## Overview

Given an approved domain spec, create the full `src/dotnet/Domains/<Name>/` structure.

**Prerequisite:** approved spec from `memeapp-domain-spec`. Do NOT start without one.

## Project Structure

```
src/dotnet/Domains/<Name>/
├── MemeApp.<Name>.Contracts/
│   ├── MemeApp.<Name>.Contracts.csproj
│   ├── <PrimaryAggregate>/
│   │   ├── Models/
│   │   │   ├── <PrimaryAggregate>.cs
│   │   │   └── <PrimaryAggregate>Id.cs
│   │   └── Services/
│   │       ├── I<PrimaryAggregate>BackendService.cs
│   │       ├── Commands/
│   │       │   ├── Add<PrimaryAggregate>Command.cs
│   │       │   ├── Update<PrimaryAggregate>Command.cs
│   │       │   └── Delete<PrimaryAggregate>Command.cs
│   │       ├── Queries/       (optional: only if custom query input models needed)
│   │       └── Events/        (optional: only if spec lists domain events)
│   │           └── <PrimaryAggregate>AddedEvent.cs
│   └── <RelatedAggregate>/
│       ├── Models/
│       └── Services/
│           ├── I<RelatedAggregate>BackendService.cs
│           ├── Commands/
│           └── Events/        (optional)
│
└── MemeApp.<Name>.Core/
    ├── MemeApp.<Name>.Core.csproj
    ├── Database/
    │   ├── Context/
    │   │   ├── <Name>DbContext.cs
    │   │   ├── <Name>DbContextMigrator.cs
    │   │   └── <Name>DbContextSettings.cs
    │   ├── Projections/
    │   │   ├── Db<PrimaryAggregate>.cs
    │   │   ├── Db<RelatedAggregate>.cs
    │   │   └── Db<Left><Right>.cs     (join table per many-to-many)
    │   ├── Extensions/    (optional)
    │   ├── Seed/          (optional)
    │   └── DatabaseModule.cs
    ├── <PrimaryAggregate>/
    │   ├── <PrimaryAggregate>BackendService.cs
    │   └── <PrimaryAggregate>Module.cs
    ├── <RelatedAggregate>/
    │   ├── <RelatedAggregate>BackendService.cs
    │   └── <RelatedAggregate>Module.cs
    └── <Name>Module.cs
```

## .csproj Files

**Contracts** — references `MemeApp.Common` (brings in Fusion transitively):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\..\..\Shared\MemeApp.Common\MemeApp.Common.csproj" />
  </ItemGroup>
</Project>
```

**Core** — implementation, DB, DI. Generators with `PrivateAssets="all"`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="ActualLab.Generators" PrivateAssets="all" />
    <PackageReference Include="MemoryPack.Generator" PrivateAssets="all" />
    <PackageReference Include="ActualLab.Fusion.EntityFramework.Npgsql" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\..\..\Shared\MemeApp.Common\MemeApp.Common.csproj" />
    <ProjectReference Include="..\MemeApp.<Name>.Contracts\MemeApp.<Name>.Contracts.csproj" />
  </ItemGroup>
</Project>
```

> If `MemoryPack.Generator` is not in `Directory.Packages.props`, add it there first.

No version numbers in .csproj — all centrally managed in `Directory.Packages.props`.

## MemeApp.slnx Entry

```xml
<Folder Name="/src/dotnet/Domains/<Name>/">
  <Project Path="src/dotnet/Domains/<Name>/MemeApp.<Name>.Contracts/MemeApp.<Name>.Contracts.csproj" />
  <Project Path="src/dotnet/Domains/<Name>/MemeApp.<Name>.Core/MemeApp.<Name>.Core.csproj" />
</Folder>
```

Also add knowledge folder entry:

```xml
<Folder Name="/.claude/knowledge/project/domains/<name>/">
  <File Path=".claude/knowledge/project/domains/<name>/README.md" />
</Folder>
```

## Knowledge Update

Add to `.claude/knowledge/project/README.md`:

```markdown
| [domains/<name>/README.md](domains/<name>/README.md) | <Name> domain spec — aggregates, operations |
```

## Key Naming Rules

| Artifact | Pattern | Example |
|----------|---------|---------|
| Domain folder | PascalCase plural noun | `Memes` |
| Project | `MemeApp.<Name>.Contracts` / `.Core` | `MemeApp.Memes.Contracts` |
| Interface | `I<Aggregate>BackendService` | `IMemeBackendService` |
| Implementation | `<Aggregate>BackendService` | `MemeBackendService` |
| Aggregate model | `<Aggregate>` (no Dto suffix) | `Meme` |
| Strongly typed ID | `<Aggregate>Id` | `MemeId` |
| Command | `<Verb><Aggregate>Command` | `AddMemeCommand` |
| Event | `<Aggregate><Verb>Event` | `MemeAddedEvent` |
| DB projection | `Db<Aggregate>` | `DbMeme` |
| Join table | `Db<Left><Right>` | `DbMemeTag` |
| DbContext | `<Name>DbContext` | `MemesDbContext` |
| Database DI module | `DatabaseModule` | `DatabaseModule` |
| Aggregate DI module | `<Aggregate>Module` | `MemeModule` |
| Root DI module | `<Name>Module` | `MemesModule` |

## Aggregate Model + ID

```csharp
// Contracts/<Aggregate>/Models/<Aggregate>.cs
public record <Aggregate>(
    <Aggregate>Id Id,
    string? Title,
    MediaType MediaType,
    DateTime CreatedAt
);

// Contracts/<Aggregate>/Models/<Aggregate>Id.cs
public readonly record struct <Aggregate>Id(Guid Value)
{
    public static <Aggregate>Id New() => new(Guid.NewGuid());
}
```

## Commands

```csharp
// Commands/Add<Aggregate>Command.cs
public record Add<Aggregate>Command(
    string? Title,
    MediaType MediaType
) : ICommand<<Aggregate>>;

// Commands/Update<Aggregate>Command.cs
public record Update<Aggregate>Command(
    <Aggregate>Id Id,
    string? Title
) : ICommand<<Aggregate>>;

// Commands/Delete<Aggregate>Command.cs
public record Delete<Aggregate>Command(<Aggregate>Id Id) : ICommand;
```

## EventCommand

```csharp
// Events/<Aggregate>AddedEvent.cs
public record <Aggregate>AddedEvent(<Aggregate>Id Id) : EventCommand, INotifyCommand;
```

Raised inside command handlers via `EnqueueAfterCompletion()` — fires after transaction commits.

## Interface Contract

`[CommandHandler]` goes on the interface, not the implementation:

```csharp
// Contracts/<Aggregate>/Services/I<Aggregate>BackendService.cs
public interface I<Aggregate>BackendService : IComputeService, IBackendService
{
    [ComputeMethod]
    Task<<Aggregate>?> GetAsync(<Aggregate>Id id, CancellationToken cancellationToken);

    [ComputeMethod]
    Task<ImmutableList<<Aggregate>>> ListAsync(CancellationToken cancellationToken);

    [CommandHandler]
    Task<<Aggregate>> AddAsync(Add<Aggregate>Command command, CancellationToken cancellationToken);

    [CommandHandler]
    Task<<Aggregate>> UpdateAsync(Update<Aggregate>Command command, CancellationToken cancellationToken);

    [CommandHandler]
    Task DeleteAsync(Delete<Aggregate>Command command, CancellationToken cancellationToken);
}
```

## Implementation

All methods (`[ComputeMethod]` and `[CommandHandler]`) must be `virtual`:

```csharp
// Core/<Aggregate>/<Aggregate>BackendService.cs
public class <Aggregate>BackendService(IDbContextFactory<<Name>DbContext> dbFactory)
    : I<Aggregate>BackendService
{
    public virtual async Task<<Aggregate>?> GetAsync(<Aggregate>Id id, CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var entity = await db.<Aggregates>.FindAsync([id.Value], cancellationToken);
        return entity?.ToModel();
    }

    public virtual async Task<ImmutableList<<Aggregate>>> ListAsync(CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var entities = await db.<Aggregates>.ToListAsync(cancellationToken);
        return entities.Select(e => e.ToModel()).ToImmutableList();
    }

    public virtual async Task<<Aggregate>> AddAsync(Add<Aggregate>Command command, CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        var entity = new Db<Aggregate> { Id = <Aggregate>Id.New().Value, /* map fields */ };
        db.<Aggregates>.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        new <Aggregate>AddedEvent(new <Aggregate>Id(entity.Id)).EnqueueAfterCompletion();

        using (Invalidation.Begin())
            _ = ListAsync(default);

        return entity.ToModel();
    }

    public virtual async Task DeleteAsync(Delete<Aggregate>Command command, CancellationToken cancellationToken)
    {
        await using var db = await dbFactory.CreateDbContextAsync(cancellationToken);
        await db.<Aggregates>
            .Where(x => x.Id == command.Id.Value)
            .ExecuteDeleteAsync(cancellationToken);

        using (Invalidation.Begin())
        {
            _ = GetAsync(command.Id, default);
            _ = ListAsync(default);
        }
    }
}
```

## DB Projection

```csharp
// Core/Database/Projections/Db<Aggregate>.cs
public class Db<Aggregate>
{
    public Guid Id { get; set; }
    // flat CLR types only — no domain records/value objects
    // configure navigation properties and relations in DbContext
}
```

## DI Modules

```csharp
// Core/Database/DatabaseModule.cs
public class DatabaseModule : AppModule
{
    public override void ConfigureServices(ModuleContext ctx)
    {
        ctx.Services.AddDbContextFactory<<Name>DbContext>(options =>
            options.UseNpgsql(/* from settings */));
    }
}

// Core/<Aggregate>/<Aggregate>Module.cs
[DependsOn<DatabaseModule>]
public class <Aggregate>Module : AppModule
{
    public override void ConfigureServices(ModuleContext ctx)
    {
        ctx.FusionBuilder.AddService<I<Aggregate>BackendService, <Aggregate>BackendService>();
    }
}

// Core/<Name>Module.cs  (root)
[DependsOn<<PrimaryAggregate>Module>]
[DependsOn<<RelatedAggregate>Module>]
public class <Name>Module : AppModule { }
```

## Checklist

- [ ] `.csproj` created: Contracts (Common ref) + Core (generators + Fusion.EF.Npgsql + ProjectRefs)
- [ ] `MemoryPack.Generator` in `Directory.Packages.props` if not present
- [ ] Projects added to `MemeApp.slnx` + knowledge folder entry
- [ ] `project/README.md` updated with domain spec link
- [ ] Aggregate models: plain records, no Dto suffix
- [ ] Each interface: `IComputeService` + `IBackendService`, suffix `BackendService`
- [ ] `[CommandHandler]` on interface methods (not implementation)
- [ ] All methods `virtual` in implementation (both `[ComputeMethod]` and `[CommandHandler]`)
- [ ] Commands: typed records implementing `ICommand<T>` or `ICommand`
- [ ] Events: `EventCommand, INotifyCommand` — only if spec lists them
- [ ] `Invalidation.Begin()` in every mutation
- [ ] `ctx.FusionBuilder.AddService<>()` — not `ctx.Services.AddFusion()`
- [ ] DI modules: `DatabaseModule`, `<Aggregate>Module`, `<Name>Module`
- [ ] `<Aggregate>Module`: `[DependsOn<DatabaseModule>]`
- [ ] `<Name>Module`: depends on all aggregate modules
- [ ] `IDbContextFactory<<Name>DbContext>` — domain-specific, not generic AppDbContext
- [ ] `CancellationToken` last, no `= default`
