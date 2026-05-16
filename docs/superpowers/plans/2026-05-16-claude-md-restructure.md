# CLAUDE.md Restructure Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace single `CLAUDE.md` with a navigable knowledge base under `.claude/knowledge/`, split by project, architecture, and code-style domains.

**Architecture:** Minimal root `CLAUDE.md` acts as project overview + table of contents. Three category folders (`project/`, `architecture/`, `code-style/`) each have a `README.md` index and individual topic files. Load only what's needed per task.

**Tech Stack:** Markdown, Claude Code `@file` references, `.gitignore`

---

## File Map

| Action | Path |
|--------|------|
| Modify | `CLAUDE.md` |
| Modify | `.gitignore` |
| Create | `.claude/knowledge/project/README.md` |
| Create | `.claude/knowledge/project/overview.md` |
| Create | `.claude/knowledge/project/domain.md` |
| Create | `.claude/knowledge/architecture/README.md` |
| Create | `.claude/knowledge/architecture/overview.md` |
| Create | `.claude/knowledge/architecture/fusion.md` |
| Create | `.claude/knowledge/code-style/README.md` |
| Create | `.claude/knowledge/code-style/naming.md` |
| Create | `.claude/knowledge/code-style/file-namespace.md` |
| Create | `.claude/knowledge/code-style/di-modules.md` |
| Create | `.claude/knowledge/code-style/fusion.md` |
| Create | `.claude/knowledge/code-style/async.md` |
| Create | `.claude/knowledge/code-style/formatting.md` |
| Create | `.claude/knowledge/code-style/blazor-ui.md` |
| Create | `.claude/knowledge/code-style/testing.md` |

---

## Task 1: Gitignore — exclude superpowers plugin output

**Files:**
- Modify: `.gitignore`

- [ ] **Step 1: Add superpowers dir to .gitignore**

Append to the end of `.gitignore`:

```
# Superpowers plugin generated output
docs/superpowers/
```

- [ ] **Step 2: Commit**

```bash
git add .gitignore
git commit -m "chore: ignore superpowers plugin output dir"
```

---

## Task 2: Rewrite root CLAUDE.md

**Files:**
- Modify: `CLAUDE.md`

- [ ] **Step 1: Replace CLAUDE.md content**

```markdown
# CLAUDE.md

This file provides guidance to Claude Code when working with this repository.

## Project

MemeApp is a personal meme library — a system for collecting, organizing, and sorting memes.
Supported media types: **Photo** (static images), **Video** (mp4/webm), **Gif** (animated).
Targets two platforms: **Web** (Blazor) and **Mobile** (MAUI).

## Build Commands

```powershell
dotnet build          # Build the solution
dotnet restore        # Restore NuGet packages
dotnet clean          # Clean build outputs
```

The solution file is `MemeApp.slnx` (modern format). No test projects yet.

## Package Management

All NuGet versions are centrally managed in `Directory.Packages.props`.
Do **not** include version numbers in individual `.csproj` files.
Add new packages to `Directory.Packages.props` first, then reference without a version.

MAUI projects are exempt from some centrally managed packages (`NU1009`);
the `.Maui` project name suffix triggers the conditional exclusion already in place.

## Knowledge Base

Load relevant docs at the start of each task:

| Context | Index |
|---------|-------|
| Business domain, product goals | `.claude/knowledge/project/README.md` |
| Solution structure, layers, libraries | `.claude/knowledge/architecture/README.md` |
| Naming, formatting, patterns | `.claude/knowledge/code-style/README.md` |
```

- [ ] **Step 2: Commit**

```bash
git add CLAUDE.md
git commit -m "docs(claude): rewrite root CLAUDE.md — project description + knowledge base TOC"
```

---

## Task 3: Project knowledge files

**Files:**
- Create: `.claude/knowledge/project/README.md`
- Create: `.claude/knowledge/project/overview.md`
- Create: `.claude/knowledge/project/domain.md`

- [ ] **Step 1: Create project/README.md**

```markdown
# Project Knowledge

| File | Contents |
|------|----------|
| [overview.md](overview.md) | Business goals, scope, target users |
| [domain.md](domain.md) | Domain entities: Meme, Tag, Collection, MediaType |
```

- [ ] **Step 2: Create project/overview.md**

```markdown
# Product Overview

## What is MemeApp?

A personal meme library. Users collect memes from the internet (or upload locally),
organize them with tags and collections, and search/filter their library.

## Media Types

| Type | Description |
|------|-------------|
| Photo | Static image (jpg, png, webp) |
| Video | Short clip (mp4, webm) |
| Gif | Animated image (gif) |

## Core Features

- Upload or link memes
- Tag memes with free-form labels
- Group into named Collections
- Search and filter by tag, type, date

## Target Platforms

- **Web** — Blazor WASM or Server (MudBlazor UI)
- **Mobile** — .NET MAUI (iOS, Android)

## Non-Goals (current scope)

- Social sharing
- Multi-user / collaboration
- AI-based auto-tagging (future)
```

- [ ] **Step 3: Create project/domain.md**

```markdown
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
| Tags | ICollection<Tag> | Many-to-many |
| CreatedAt | DateTime | UTC |

### MediaType
```csharp
public enum MediaType { Photo, Video, Gif }
```

### Tag
Flat label for categorization. No hierarchy.

| Property | Type |
|----------|------|
| Id | Guid |
| Name | string |
| Slug | string | URL-safe, lowercase, unique |

### Collection
Named group of memes. One meme can appear in many collections.

| Property | Type |
|----------|------|
| Id | Guid |
| Name | string |
| Description | string? |
| Memes | ICollection<Meme> |

## Relationships

```
Meme ──< MemeTag >── Tag
Meme ──< CollectionMeme >── Collection
```
```

- [ ] **Step 4: Commit**

```bash
git add .claude/knowledge/project/
git commit -m "docs(claude): add project knowledge — overview and domain model"
```

---

## Task 4: Architecture knowledge files

**Files:**
- Create: `.claude/knowledge/architecture/README.md`
- Create: `.claude/knowledge/architecture/overview.md`
- Create: `.claude/knowledge/architecture/fusion.md`

- [ ] **Step 1: Create architecture/README.md**

```markdown
# Architecture Knowledge

| File | Contents |
|------|----------|
| [overview.md](overview.md) | Solution layers, bounded contexts, key libraries |
| [fusion.md](fusion.md) | ActualLab.Fusion patterns used in this project |
```

- [ ] **Step 2: Create architecture/overview.md**

```markdown
# Architecture Overview

## Solution Structure

```
src/dotnet/
  Apps/
    MemeApp.Apps.UI.Core/       ← Blazor/MAUI shared UI host
  Core/
    MemeApp.Core/               ← Cross-cutting: DI module system, shared config
    Users/
      Contracts/                ← IUserService, UserDto — no impl deps
      Services/                 ← UserService implementations
    MemeData/
      Contracts/                ← IMemeService, MemeDto, ITagService, etc.
      Services/                 ← Implementations
  Infrastructure/
    MemeApp.Infrastructure.Data/ ← EF Core, migrations, DB context
  Shared/
    MemeApp.Common/             ← Utilities: LinqExt, host module system
    MemeApp.Common.Tests/       ← Tests for Common
```

## Layer Rules

- **Contracts** projects: interfaces + DTOs only. No implementation references.
- **Services** projects: implement Contracts. May reference Infrastructure.
- **Apps** projects: wire up DI, host. Reference everything.
- **Shared**: no domain knowledge. Pure utilities.

## Bounded Context Pattern

Each bounded context under `Core/` has:
- `Contracts/` — public surface (interfaces, DTOs, enums)
- `Services/` — implementations behind those interfaces

Consumer projects reference only `Contracts`. This keeps build graphs clean.

## DI Registration

Each project exposes a static `Configure` class.
`AddModules(...)` in `MemeApp.Common` wires them up with topological sort + cycle detection.

## Key Libraries

| Library | Purpose |
|---------|---------|
| **ActualLab.Fusion** | Real-time reactive state: computed services, live queries, sessions |
| **MudBlazor** | Material Design UI components for Blazor |
| **FluentValidation** | Input validation |
| **Markdig + Markdown.ColorCode** | Markdown rendering with syntax highlighting |
| **EF Core** | Data access (Infrastructure layer) |
```

- [ ] **Step 3: Create architecture/fusion.md**

```markdown
# ActualLab.Fusion Architecture

## What Fusion Does

Fusion provides transparent reactive caching.
A `[ComputeMethod]` result is cached and automatically invalidated
when its dependencies change — clients get live updates without polling.

## IComputeService

All Fusion-reactive services implement `IComputeService`:

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default);

    [ComputeMethod]
    Task<ImmutableList<MemeDto>> ListByTagAsync(string tag, CancellationToken ct = default);
}
```

Rules:
- Interface must extend `IComputeService`
- Reactive methods get `[ComputeMethod]` attribute
- Return type must be `Task<T>` (not `ValueTask`)
- Last parameter is always `CancellationToken ct = default`

## Invalidation

Mutations invalidate affected compute results:

```csharp
public async Task AddTagAsync(Guid memeId, string tag, CancellationToken ct = default)
{
    // ... persist ...
    using (Computed.Invalidate())
    {
        _ = GetAsync(memeId);
        _ = ListByTagAsync(tag);
    }
}
```

## Blazor Integration

Use `ComputedState` or `MixedStateFactory` in Blazor components to bind to live data:

```csharp
[CascadingParameter] public Task<AuthState> AuthStateTask { get; set; } = null!;

protected override async Task OnInitializedAsync()
{
    State = await StateFactory.NewComputed<ImmutableList<MemeDto>>(
        new() { UpdateDelayer = FixedDelayer.Instant },
        async (_, ct) => await MemeService.ListByTagAsync(CurrentTag, ct));
}
```

## Session / Auth

- `IAuth` provides session-based authentication
- `Session` flows through service method parameters
- Never store session in a field — always pass through the call chain
```

- [ ] **Step 4: Commit**

```bash
git add .claude/knowledge/architecture/
git commit -m "docs(claude): add architecture knowledge — overview and Fusion patterns"
```

---

## Task 5: Code style — README + formatting

**Files:**
- Create: `.claude/knowledge/code-style/README.md`
- Create: `.claude/knowledge/code-style/formatting.md`

- [ ] **Step 1: Create code-style/README.md**

```markdown
# Code Style

| File | Contents |
|------|----------|
| [formatting.md](formatting.md) | Braces, indentation, expression bodies, var, line endings |
| [naming.md](naming.md) | Types, interfaces, DTOs, methods, fields, locals |
| [file-namespace.md](file-namespace.md) | File-scoped namespaces, one type per file, folder matching |
| [di-modules.md](di-modules.md) | DI registration, Configure class, AddModules |
| [fusion.md](fusion.md) | Fusion-specific coding patterns |
| [async.md](async.md) | async/await, CancellationToken, Task vs ValueTask |
| [blazor-ui.md](blazor-ui.md) | Blazor components, MudBlazor, Fusion integration |
| [testing.md](testing.md) | Test structure, naming, what to test |
```

- [ ] **Step 2: Create code-style/formatting.md**

Source of truth: `.editorconfig` at repo root.

```markdown
# Formatting

## Braces — Allman style

Opening brace on new line for: types, methods, control blocks, properties, accessors, lambdas.

```csharp
// Correct
public class MemeService
{
    public async Task<MemeDto?> GetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return null;
        }
        return await _repo.FindAsync(id);
    }
}
```

Exception: no braces preferred for single-statement blocks (`csharp_prefer_braces = false`).

```csharp
if (id == Guid.Empty)
    return null;
```

## Expression Bodies

| Member kind | Style |
|-------------|-------|
| Methods | Expression-bodied preferred |
| Properties | Expression-bodied preferred |
| Constructors | Block body required |
| Accessors (`get`/`set`) | Block body required |

```csharp
// Method — expression body
public string GetSlug() => Name.ToLowerInvariant().Replace(' ', '-');

// Property — expression body
public string DisplayName => $"{Name} ({MediaType})";

// Constructor — block body
public MemeService(IRepository repo)
{
    _repo = repo;
}
```

## var vs Explicit Type

| Case | Rule |
|------|------|
| Type apparent from RHS | `var` |
| Built-in types (`int`, `string`, `bool`, etc.) | Explicit |
| Complex generic or LINQ result | `var` |

```csharp
var meme = await _repo.FindAsync(id);   // type apparent
int count = memes.Count;                // built-in
string name = tag.Name;                 // built-in
var results = memes.Where(m => ...).ToList();  // complex
```

## Indentation & Line Endings

- 4 spaces (no tabs) for `.cs` files
- CRLF line endings
- 2 spaces for `.json`, `.xml`, `.csproj`, `.props`

## Modifiers Order

```
public private internal protected async static readonly sealed override abstract
```

## Other

- No `this.` qualification on any member
- No primary constructors (`csharp_style_prefer_primary_constructors = false`)
- File-scoped namespaces (see file-namespace.md)
- `using` directives outside namespace
```

- [ ] **Step 3: Commit**

```bash
git add .claude/knowledge/code-style/README.md .claude/knowledge/code-style/formatting.md
git commit -m "docs(claude): add code-style README and formatting rules"
```

---

## Task 6: Code style — naming + file/namespace

**Files:**
- Create: `.claude/knowledge/code-style/naming.md`
- Create: `.claude/knowledge/code-style/file-namespace.md`

- [ ] **Step 1: Create code-style/naming.md**

```markdown
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
| Parameter | camelCase | `memeId`, `cancellationToken` / `ct` |
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

## No Abbreviations

Full words preferred. Exception: well-known abbreviations (`ct` for `CancellationToken`, `dto` for DTO locals).
```

- [ ] **Step 2: Create code-style/file-namespace.md**

```markdown
# File and Namespace Organization

## File-Scoped Namespaces

Always use file-scoped namespace declaration:

```csharp
namespace MemeApp.Core.MemeData.Services;

public class MemeService : IMemeService
{
    // ...
}
```

Never:
```csharp
namespace MemeApp.Core.MemeData.Services
{
    public class MemeService : IMemeService { }
}
```

## One Type Per File

Each `.cs` file contains exactly one top-level type.
File name matches type name exactly:

```
MemeService.cs       → public class MemeService
IMemeService.cs      → public interface IMemeService
MemeDto.cs           → public record MemeDto
```

## Namespace Matches Folder

Namespace mirrors the folder path from the project root:

```
src/dotnet/Core/MemeData/Services/MemeService.cs
→ namespace MemeApp.Core.MemeData.Services;
```

## Using Directives

Outside namespace (file-scoped style enforces this).
No forced ordering of `System.*` vs others.

```csharp
using System.Collections.Immutable;
using ActualLab.Fusion;
using MemeApp.Core.MemeData.Contracts;

namespace MemeApp.Core.MemeData.Services;
```
```

- [ ] **Step 3: Commit**

```bash
git add .claude/knowledge/code-style/naming.md .claude/knowledge/code-style/file-namespace.md
git commit -m "docs(claude): add naming and file/namespace code style rules"
```

---

## Task 7: Code style — DI + Fusion patterns

**Files:**
- Create: `.claude/knowledge/code-style/di-modules.md`
- Create: `.claude/knowledge/code-style/fusion.md`

- [ ] **Step 1: Create code-style/di-modules.md**

```markdown
# DI and Module Registration

## Configure Static Class

Every project exposes a `Configure` static class as the DI entry point.
Methods are extension methods on `IServiceCollection`.

```csharp
namespace MemeApp.Core.MemeData.Services;

public static class Configure
{
    public static IServiceCollection AddMemeDataServices(this IServiceCollection services)
    {
        services.AddSingleton<IMemeService, MemeService>();
        services.AddSingleton<ITagService, TagService>();
        return services;
    }
}
```

## AddModules

`AddModules` in `MemeApp.Common` wires up all modules with topological sort and cycle detection.
Declare module dependencies explicitly via the module graph — do not manually order calls.

```csharp
services.AddModules(modules =>
{
    modules.Add<MemeDataModule>();
    modules.Add<UsersModule>();
    modules.Add<InfrastructureModule>();
});
```

## Rules

- No logic in `Configure` — registration only
- No `new` on services — always registered and resolved via DI
- Prefer `AddSingleton` for stateless services, `AddScoped` for per-request state
- Fusion services registered via `fusion.AddService<TInterface, TImpl>()`
```

- [ ] **Step 2: Create code-style/fusion.md**

```markdown
# Fusion Coding Patterns

See also: `.claude/knowledge/architecture/fusion.md` for architectural overview.

## Service Interface

```csharp
public interface IMemeService : IComputeService
{
    [ComputeMethod]
    Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default);

    [ComputeMethod]
    Task<ImmutableList<MemeDto>> ListByTagAsync(string tag, CancellationToken ct = default);
}
```

## Service Implementation

```csharp
public class MemeService(IDbContextFactory<AppDbContext> dbFactory) : IMemeService
{
    public virtual async Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var meme = await db.Memes.FindAsync([id], ct);
        return meme?.ToDto();
    }

    public virtual async Task<ImmutableList<MemeDto>> ListByTagAsync(
        string tag, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return (await db.Memes
            .Where(m => m.Tags.Any(t => t.Slug == tag))
            .ToListAsync(ct))
            .Select(m => m.ToDto())
            .ToImmutableList();
    }
}
```

## Rules

- Computed methods must be `virtual` (Fusion proxies override them)
- Return type: `Task<T>` — not `ValueTask`, not `T`
- `CancellationToken ct = default` is always the last parameter
- Mutations call `Computed.Invalidate()` after persisting changes
- Never store mutable state in a Fusion service — all state goes to DB

## Invalidation Pattern

```csharp
public async Task DeleteAsync(Guid id, CancellationToken ct = default)
{
    await using var db = await dbFactory.CreateDbContextAsync(ct);
    await db.Memes.Where(m => m.Id == id).ExecuteDeleteAsync(ct);

    using (Computed.Invalidate())
    {
        _ = GetAsync(id);
        // invalidate any list queries that would include this meme
    }
}
```

## Registration

```csharp
// In Configure static class
var fusion = services.AddFusion();
fusion.AddService<IMemeService, MemeService>();
```
```

- [ ] **Step 3: Commit**

```bash
git add .claude/knowledge/code-style/di-modules.md .claude/knowledge/code-style/fusion.md
git commit -m "docs(claude): add DI/modules and Fusion code style rules"
```

---

## Task 8: Code style — async + Blazor/UI

**Files:**
- Create: `.claude/knowledge/code-style/async.md`
- Create: `.claude/knowledge/code-style/blazor-ui.md`

- [ ] **Step 1: Create code-style/async.md**

```markdown
# Async Patterns

## Method Signatures

All service methods are async. `CancellationToken ct = default` is always last:

```csharp
Task<MemeDto?> GetAsync(Guid id, CancellationToken ct = default);
Task AddTagAsync(Guid memeId, string tagSlug, CancellationToken ct = default);
```

## Task vs ValueTask

- Use `Task<T>` for Fusion compute methods (required by Fusion)
- Use `ValueTask<T>` for hot paths that frequently return synchronously (e.g., cache hits)
- Default to `Task<T>` unless profiling shows ValueTask is worth it

## Never Block

Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` on tasks.

```csharp
// Wrong
var meme = _service.GetAsync(id).Result;

// Correct
var meme = await _service.GetAsync(id);
```

## ConfigureAwait

No `ConfigureAwait(false)` in application code (only in library code).
This project is application-layer, not a library.

## Parallel Work

Use `Task.WhenAll` for independent concurrent operations:

```csharp
var (meme, tags) = await (
    memeService.GetAsync(id, ct),
    tagService.ListForMemeAsync(id, ct)
).WhenAll();  // or Task.WhenAll + destructure
```
```

- [ ] **Step 2: Create code-style/blazor-ui.md**

```markdown
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

Use `ComputedState<T>` for reactive data binding:

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
```

- [ ] **Step 3: Commit**

```bash
git add .claude/knowledge/code-style/async.md .claude/knowledge/code-style/blazor-ui.md
git commit -m "docs(claude): add async and Blazor/UI code style rules"
```

---

## Task 9: Code style — testing

**Files:**
- Create: `.claude/knowledge/code-style/testing.md`

- [ ] **Step 1: Create code-style/testing.md**

```markdown
# Testing

No test projects exist yet (except `MemeApp.Common.Tests`).
This file documents the conventions to follow when tests are added.

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
```

- [ ] **Step 2: Commit**

```bash
git add .claude/knowledge/code-style/testing.md
git commit -m "docs(claude): add testing conventions"
```

---

## Task 10: Verify structure

- [ ] **Step 1: Verify all files exist**

```powershell
Get-ChildItem .claude/knowledge -Recurse -Filter "*.md" | Select-Object FullName
```

Expected output — 14 files:
```
.claude/knowledge/project/README.md
.claude/knowledge/project/overview.md
.claude/knowledge/project/domain.md
.claude/knowledge/architecture/README.md
.claude/knowledge/architecture/overview.md
.claude/knowledge/architecture/fusion.md
.claude/knowledge/code-style/README.md
.claude/knowledge/code-style/naming.md
.claude/knowledge/code-style/file-namespace.md
.claude/knowledge/code-style/di-modules.md
.claude/knowledge/code-style/fusion.md
.claude/knowledge/code-style/async.md
.claude/knowledge/code-style/formatting.md
.claude/knowledge/code-style/blazor-ui.md
.claude/knowledge/code-style/testing.md
```

- [ ] **Step 2: Verify root CLAUDE.md has no ErrorOr / Fluxor references**

```powershell
Select-String -Path CLAUDE.md -Pattern "ErrorOr|Fluxor"
```

Expected: no matches.

- [ ] **Step 3: Verify docs/superpowers/ is in .gitignore**

```powershell
git check-ignore -v docs/superpowers/specs/
```

Expected: `.gitignore:... docs/superpowers/`
