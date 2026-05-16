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

Never block-scoped:

```csharp
// Wrong
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
No forced ordering of `System.*` vs others (`dotnet_sort_system_directives_first = false`).

```csharp
using System.Collections.Immutable;
using ActualLab.Fusion;
using MemeApp.Core.MemeData.Contracts;

namespace MemeApp.Core.MemeData.Services;
```
