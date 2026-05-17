using ActualLab.CommandR.Configuration;

namespace MemeApp.Common;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class EventHandlerAttribute : CommandHandlerAttribute;
