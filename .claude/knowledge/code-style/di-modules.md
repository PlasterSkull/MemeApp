# DI and Module Registration

## AddModules

All modules registered via `AddModules` in `MemeApp.Common`.
Topological sort + cycle detection handle ordering automatically.
Never manually order module registration calls.

```csharp
services.AddModules(modules =>
{
    modules.Add<MemeDataModule>();
    modules.Add<UsersModule>();
    modules.Add<InfrastructureModule>();
});
```

## Rules

- No `new` on services — always registered and resolved via DI
- Prefer `AddSingleton` for stateless services, `AddScoped` for per-request state
- Fusion services registered via `fusion.AddService<TInterface, TImpl>()`
