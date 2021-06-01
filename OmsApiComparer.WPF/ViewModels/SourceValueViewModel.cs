using ApiComparer;

namespace OmsApiComparer.WPF
{
    public record SourceValueViewModel(
        string Source,
        NormalizedProperty Value
        );
}
