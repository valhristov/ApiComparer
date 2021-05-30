using System.Collections.Immutable;
using System.Linq;
using ApiComparer;

namespace OmsApiComparer.WPF
{
    public record ComparisonViewModel<T>(
        string Name ,
        SourceViewModel<T> A,
        SourceViewModel<T> B,
        SourceViewModel<T> C,
        SourceViewModel<T> D
        );

    public record SourceViewModel<T>(
        string Source,
        T Value
        );

    public record RequestComparisonViewModel(
        string MethodAndPath,
        ImmutableArray<ComparisonViewModel<NormalizedProperty>> QueryString,
        ImmutableArray<ComparisonViewModel<NormalizedProperty>> Headers
        );

}
