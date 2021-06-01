using ApiComparer;

namespace OmsApiComparer.WPF
{
    public record PropertyWithSourceViewModel(
        string Source,
        NormalizedProperty Value
        );
}
