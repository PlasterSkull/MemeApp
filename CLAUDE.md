# CLAUDE.md

This file provides guidance to Claude Code when working with this repository.

## Project

MemeApp is a personal meme library — a system for collecting, organizing, and sorting memes.
Supported media types: **Photo** (static images), **Video** (mp4/webm), **Gif** (animated).
Targets two platforms: **Web** (Blazor) and **Mobile** (MAUI with Blazor).

## Build Commands

```powershell
dotnet build          # Build the solution
dotnet restore        # Restore NuGet packages
dotnet clean          # Clean build outputs
```

The solution file is `MemeApp.slnx` (modern format). No test projects yet.

## Knowledge Base

| Context | Index | When to load |
|---------|-------|--------------|
| Business domain, product goals | `.claude/knowledge/project/README.md` | Discussing features, domain model, product decisions |
| Solution structure, layers, libraries, packages | `.claude/knowledge/architecture/README.md` | Adding projects, wiring dependencies, choosing libraries |
| Naming, formatting, patterns | `.claude/knowledge/code-style/README.md` | Writing or reviewing any C# code |
