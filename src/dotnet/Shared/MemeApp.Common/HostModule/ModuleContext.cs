using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MemeApp.Common.HostModule;

public sealed class ModuleContext
{
    public IServiceCollection Services { get; }
    public IConfiguration Configuration { get; }
    public IHostEnvironment HostEnvironment { get; }
    public IReadOnlyList<Assembly> ModuleAssemblies { get; }

    public ModuleContext(
        IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment,
        IReadOnlyList<Assembly> moduleAssemblies)
    {
        Services = services;
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
        ModuleAssemblies = moduleAssemblies;
    }
}
