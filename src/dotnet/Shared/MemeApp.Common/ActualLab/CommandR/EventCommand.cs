using ActualLab.CommandR;

namespace MemeApp.Common;

public abstract partial record EventCommand : IEventCommand
{
    public string ChainId { get; init; } = string.Empty;
}
