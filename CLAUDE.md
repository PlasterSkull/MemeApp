# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```powershell
dotnet build          # Build the solution
dotnet restore        # Restore NuGet packages
dotnet clean          # Clean build outputs
```

There are no test projects yet. The solution file is `MemeApp.slnx` (modern format).

## Architecture

This is a .NET 9 cross-platform app targeting both Blazor (web) and MAUI (mobile). The solution lives under `src/dotnet/` and is organized into three layers:

- **`src/dotnet/Apps/`** — Application entry points (Blazor/MAUI hosts). Currently empty; apps go here.
- **`src/dotnet/Core/`** — Domain logic split into bounded contexts:
  - `MemeApp.Core` — Cross-cutting core configuration
  - `Core/Users/` — User management (`Contracts`, `Services`, `Migrations`)
  - `Core/MemeData/` — Meme content (`Contracts`, `Services`)
- **`src/dotnet/Shared/MemeApp.Common/`** — Shared utilities

Within each bounded context, **Contracts** projects hold interfaces and DTOs; **Services** projects hold implementations. This separation keeps consumer projects from depending on implementation details.

Each project exposes a `Configure` static class intended as the DI registration entry point.

## Key Libraries

- **MudBlazor** — Material Design UI components
- **Fluxor** — Redux-style state management for Blazor
- **ActualLab.Fusion** — Real-time reactive state (computed services, live queries)
- **ErrorOr** — Railway-oriented error handling (prefer `ErrorOr<T>` return types over exceptions in service layer)
- **FluentValidation** — Input validation
- **Markdig + Markdown.ColorCode** — Markdown rendering with syntax highlighting

## Package Management

All NuGet versions are centrally managed in `Directory.Packages.props` — do **not** include version numbers in individual `.csproj` files. Add new packages to `Directory.Packages.props` first, then reference them without a version.

MAUI projects are exempt from some centrally managed packages due to a known SDK incompatibility (`NU1009`); the `.Maui` project name suffix triggers the conditional exclusion already in place.

## Code Style

The `.editorconfig` enforces these non-default preferences:
- Allman braces (opening brace on new line) for types, methods, control blocks
- `var` preferred when type is apparent; explicit types for built-in types (`int`, `string`, etc.)
- Expression-bodied members preferred for methods and properties; block bodies preferred for constructors and accessors
- File-scoped namespaces
- No `this.` qualification
- Primary constructors are **not** preferred (`csharp_style_prefer_primary_constructors = false`)
