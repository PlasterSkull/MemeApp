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
