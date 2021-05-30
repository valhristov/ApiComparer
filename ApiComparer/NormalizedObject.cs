using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ApiComparer
{
    public record NormalizedObject(
        string Name,
        ImmutableArray<NormalizedProperty> Properties
        );
}
