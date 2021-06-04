using System.Collections.Immutable;
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

        public bool IsCustomType =>
            Property?.Type != null && !_types.Contains(Property.Type);

        private static readonly ImmutableHashSet<string> _types =
            ImmutableHashSet.Create("string", "integer", "boolean", "array");
    }
}
