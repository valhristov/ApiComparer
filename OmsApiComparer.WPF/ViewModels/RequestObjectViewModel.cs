using System.Collections.Immutable;

namespace OmsApiComparer.WPF
{
    public record RequestObjectViewModel(
        string Name,
        ImmutableArray<ComparisonViewModel> Properties
        );
}
