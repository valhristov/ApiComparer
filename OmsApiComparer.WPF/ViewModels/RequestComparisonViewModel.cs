using System.Collections.Immutable;

namespace OmsApiComparer.WPF
{
    public record RequestComparisonViewModel(
        string MethodAndPath,
        ImmutableArray<ComparisonViewModel> QueryString,
        ImmutableArray<ComparisonViewModel> Headers,
        ImmutableArray<RequestObjectViewModel> RequestObjects
        );
}
