using System.Collections.Immutable;

namespace OmsApiComparer.WPF
{
    public record ObjectViewModel(
        string Name,
        ImmutableArray<PropertyViewModel> Properties
        );
}
