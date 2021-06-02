using ApiComparer;

namespace OmsApiComparer.WPF
{
    public record PropertyWithSourceViewModel(
        string Source,
        string Name,
        NormalizedProperty Property
        )
    {
        public object Value => Property?.Formatted;
    }
}
