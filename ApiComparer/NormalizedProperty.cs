using System.Text.Json.Serialization;

namespace ApiComparer
{
    public record NormalizedProperty(
        string Name,
        string Type,
        [property:JsonIgnore]string Description,
        bool IsRequired
        );
}
