---
name: memeapp-domain-impl
description: Use when implementing a MemeApp domain from an approved domain spec — creates project structure, csproj files, interfaces with CommandHandler, event commands, DB projections, and DI modules following MemeApp conventions.
---

# MemeApp Domain Implementation

## Overview

Given an approved domain spec, create the full `src/dotnet/Domains/<Name>/` structure.

**Prerequisite:** approved spec from `memeapp-domain-spec`. Do NOT start without one.

## Project Structure

Aggregate folders inside projects are **plural** (e.g., `Memes/`, `Tags/`, not `Meme/`, `Tag/`).

```
src/dotnet/Domains/<Name>/
├── MemeApp.<Name>.Contracts/
│   ├── MemeApp.<Name>.Contracts.csproj
│   ├── <PrimaryAggregates>/
│   │   ├── Models/
│   │   │   ├── <PrimaryAggregate>.cs
│   │   │   └── <PrimaryAggregate>Id.cs
│   │   └── Services/
│   │       ├── I<PrimaryAggregate>BackendService.cs
│   │       ├── Commands/
│   │       │   ├── Add<PrimaryAggregate>Command.cs
│   │       │   ├── Update<PrimaryAggregate>Command.cs
│   │       │   └── Delete<PrimaryAggregate>Command.cs
│   │       ├── Queries/       (optional)
│   │       └── Events/        (optional)
│   │           └── <PrimaryAggregate>AddedEvent.cs
│   └── <RelatedAggregates>/
│       ├── Models/
│       └── Services/
│           ├── I<RelatedAggregate>BackendService.cs
│           ├── Commands/
│           └── Events/        (optional)
│
└── MemeApp.<Name>.Core/
    ├── MemeApp.<Name>.Core.csproj
    ├── GlobalUsings.cs
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
    ├── <PrimaryAggregates>/
    │   ├── <PrimaryAggregate>BackendService.cs
    │   └── <PrimaryAggregate>Module.cs
    ├── <RelatedAggregates>/
    │   ├── <RelatedAggregate>BackendService.cs
    │   └── <RelatedAggregate>Module.cs
    └── <Name>Module.cs
```

## Namespaces

Single flat namespace per project — no sub-namespaces per aggregate or folder:

```csharp
// All files in MemeApp.<Name>.Contracts:
namespace MemeApp.<Name>.Contracts;

// All files in MemeApp.<Name>.Core:
namespace MemeApp.<Name>.Core;
```

## GlobalUsings.cs

Place in project root of `MemeApp.<Name>.Core`. Reduces per-file usings:

```csharp
// Core/GlobalUsings.cs
global using System.Collections.Immutable;
global using ActualLab.Fusion;
global using Microsoft.EntityFrameworkCore;
global using MemeApp.<Name>.Contracts;
```

Add more as needed for commonly used types in the domain.

## .csproj Files

**Contracts** — references `MemeApp.Common` (brings in Fusion transitively):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\..\..\..\Shared\MemeApp.Common\MemeApp.Common.csproj" />
  </ItemGroup>
</Project>
```

**Core** — generators with `PrivateAssets="all"`:

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

> If `MemoryPack.Generator` is missing from `Directory.Packages.props`, add it there first.

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
| Aggregate subfolder | PascalCase plural noun | `Memes/`, `Tags/` |
| Project | `MemeApp.<Name>.Contracts` / `.Core` | `MemeApp.Memes.Contracts` |
| Namespace | `MemeApp.<Name>.Contracts` / `.Core` | flat, no sub-namespaces |
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
namespace MemeApp.<Name>.Contracts;

public record <Aggregate>(
    <Aggregate>Id Id,
    string? Title,
    DateTime CreatedAt
);

public readonly record struct <Aggregate>Id(Guid Value)
{
    public static <Aggregate>Id New() => new(Guid.NewGuid());
}
```

## Commands

```csharp
namespace MemeApp.<Name>.Contracts;

public record Add<Aggregate>Command(string? Title) : ICommand<<Aggregate>>;
public record Update<Aggregate>Command(<Aggregate>Id Id, string? Title) : ICommand<<Aggregate>>;
public record Delete<Aggregate>Command(<Aggregate>Id Id) : ICommand;
```

## EventCommand

```csharp
namespace MemeApp.<Name>.Contracts;

public record <Aggregate>AddedEvent(<Aggregate>Id Id) : EventCommand, INotifyCommand;
```

Raised inside command handlers via `EnqueueAfterCompletion()` — fires after transaction commits.

## Interface Contract

`[CommandHandler]` goes on the interface, not the implementation:

```csharp
namespace MemeApp.<Name>.Contracts;

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

All methods must be `virtual` (both `[ComputeMethod]` and `[CommandHandler]`):

```csharp
namespace MemeApp.<Name>.Core;

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
        var entity = new Db<Aggregate> { Id = <Aggregate>Id.New().Value };
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
namespace MemeApp.<Name>.Core;

public class Db<Aggregate>
{
    public Guid Id { get; set; }
    // flat CLR types only — no domain records/value objects
    // configure navigation properties and relations in DbContext
}
```

## DI Modules

```csharp
namespace MemeApp.<Name>.Core;

// Database/DatabaseModule.cs
public class DatabaseModule : AppModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        context.Services.AddDbContextFactory<<Name>DbContext>(options =>
            options.UseNpgsql(/* from settings */));
    }
}

// <Aggregates>/<Aggregate>Module.cs
[DependsOn<DatabaseModule>]
public class <Aggregate>Module : AppModule
{
    public override void ConfigureServices(ModuleContext context)
    {
        context.FusionBuilder.AddService<I<Aggregate>BackendService, <Aggregate>BackendService>();
    }
}

// <Name>Module.cs  (root)
[DependsOn<<PrimaryAggregate>Module>]
[DependsOn<<RelatedAggregate>Module>]
public class <Name>Module : AppModule { }
```

## Checklist

- [ ] `.csproj` created: Contracts (Common ref) + Core (generators + Fusion.EF.Npgsql + ProjectRefs)
- [ ] `MemoryPack.Generator` in `Directory.Packages.props` if not present
- [ ] `GlobalUsings.cs` in Core root with common usings
- [ ] Projects added to `MemeApp.slnx` + knowledge folder entry
- [ ] `project/README.md` updated with domain spec link
- [ ] All files in Contracts: `namespace MemeApp.<Name>.Contracts;`
- [ ] All files in Core: `namespace MemeApp.<Name>.Core;`
- [ ] Aggregate subfolders are plural (`Memes/`, `Tags/`, not `Meme/`, `Tag/`)
- [ ] Aggregate models: plain records, no Dto suffix
- [ ] Each interface: `IComputeService` + `IBackendService`, suffix `BackendService`
- [ ] `[CommandHandler]` on interface methods (not implementation)
- [ ] All methods `virtual` in implementation
- [ ] Commands: typed records implementing `ICommand<T>` or `ICommand`
- [ ] Events: `EventCommand, INotifyCommand` — only if spec lists them
- [ ] `Invalidation.Begin()` in every mutation
- [ ] `context.FusionBuilder.AddService<>()` — not `ctx.Services.AddFusion()`
- [ ] DI module parameter named `context`, not `ctx`
- [ ] DI modules: `DatabaseModule`, `<Aggregate>Module`, `<Name>Module`
- [ ] `<Aggregate>Module`: `[DependsOn<DatabaseModule>]`
- [ ] `<Name>Module`: depends on all aggregate modules
- [ ] `IDbContextFactory<<Name>DbContext>` — domain-specific, not generic
- [ ] `CancellationToken` last, no `= default`
