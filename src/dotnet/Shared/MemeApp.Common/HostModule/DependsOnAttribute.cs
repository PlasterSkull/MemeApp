namespace MemeApp.Common.HostModule;

internal interface IDependsOnModule
{
    Type ModuleType { get; }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public sealed class DependsOnAttribute<TModule> : Attribute, IDependsOnModule
    where TModule : AppModule
{
    public Type ModuleType => typeof(TModule);
}
