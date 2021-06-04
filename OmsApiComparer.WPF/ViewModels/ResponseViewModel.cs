using System.Collections.Immutable;

namespace OmsApiComparer.WPF
{
    public record ResponseViewModel(
        string StatusCode,
        ImmutableArray<PropertyViewModel> Properties
        );
}
