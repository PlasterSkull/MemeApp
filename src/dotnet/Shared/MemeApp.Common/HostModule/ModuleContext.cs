using System.Reflection;
using ActualLab.Fusion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MemeApp.Common.HostModule;

public sealed record ModuleContext
{
    public required IServiceCollection Services { get; init; }
    public required IConfiguration Configuration { get; init; }
    public required IHostEnvironment HostEnvironment { get; init; }
    public required IReadOnlyList<Assembly> ModuleAssemblies { get; init; }
    public required FusionBuilder FusionBuilder { get; init; }
}
