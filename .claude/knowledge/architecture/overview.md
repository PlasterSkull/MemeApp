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

Each project exposes a static `Configure` class with registration methods.
`AddModules(...)` in `MemeApp.Common` wires them up with topological sort + cycle detection.

## Package Management

All NuGet versions are centrally managed in `Directory.Packages.props` at repo root.
Do **not** include version numbers in individual `.csproj` files.
Add new packages to `Directory.Packages.props` first, then reference without a version.

MAUI projects are exempt from some centrally managed packages (`NU1009`);
the `.Maui` project name suffix triggers the conditional exclusion already in place.

## Key Libraries

| Library | Purpose |
|---------|---------|
| **ActualLab.Fusion** | Real-time reactive state: computed services, live queries, sessions |
| **MudBlazor** | Material Design UI components for Blazor |
| **FluentValidation** | Input validation |
| **EF Core** | Data access (Infrastructure layer) |
