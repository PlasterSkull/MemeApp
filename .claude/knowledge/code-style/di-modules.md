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
