using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MemeApp.Common.HostModule;

public static class ModuleApplicationExtensions
{
    public static IServiceCollection AddModules<TRootModule>(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
        where TRootModule : AppModule
    {
        var ordered = TopologicalSort(typeof(TRootModule));

        var assemblies = ordered
            .Select(t => t.Assembly)
            .Distinct()
            .ToList()
            .AsReadOnly();

        var context = new ModuleContext(services, configuration, hostEnvironment, assemblies);

        foreach (var moduleType in ordered)
        {
            var module = (AppModule)Activator.CreateInstance(moduleType)!;
            module.ConfigureServices(context);
        }

        return services;
    }

    private static List<Type> TopologicalSort(Type rootType)
    {
        var visited = new HashSet<Type>();
        var visiting = new HashSet<Type>();
        var result = new List<Type>();

        void Visit(Type type)
        {
            if (visited.Contains(type))
                return;

            if (!visiting.Add(type))
                throw new InvalidOperationException(
                    $"Circular module dependency detected involving '{type.FullName}'.");

            foreach (var dep in GetDependencies(type))
                Visit(dep);

            visiting.Remove(type);
            visited.Add(type);
            result.Add(type);
        }

        Visit(rootType);
        return result;
    }

    private static IEnumerable<Type> GetDependencies(Type moduleType) =>
        moduleType
            .GetCustomAttributes(inherit: true)
            .OfType<IDependsOnModule>()
            .Select(a => a.ModuleType);
}
