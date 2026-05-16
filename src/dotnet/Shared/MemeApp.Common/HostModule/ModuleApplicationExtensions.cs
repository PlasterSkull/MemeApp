using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Immutable;

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

        var context = new ModuleContext
        {
            Services = services,
            Configuration = configuration,
            HostEnvironment = hostEnvironment,
            ModuleAssemblies = ordered
                .Select(t => t.Assembly)
                .Distinct()
                .ToImmutableList(),
        };

        ordered
            .Select(moduleType => Activator.CreateInstance(moduleType))
            .Cast<AppModule>()
            .ForEach(module => module.ConfigureServices(context));

        return services;
    }

    private static List<Type> TopologicalSort(Type rootType)
    {
        var visited = new HashSet<Type>();
        var visiting = new HashSet<Type>();
        var result = new List<Type>();

        Visit(rootType);

        return result;

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
    }

    private static IEnumerable<Type> GetDependencies(Type moduleType) =>
        moduleType
            .GetCustomAttributes(typeof(IDependsOnModule), inherit: true)
            .Cast<IDependsOnModule>()
            .Select(a => a.ModuleType);
}
