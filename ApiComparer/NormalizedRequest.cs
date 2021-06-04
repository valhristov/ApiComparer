using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace ApiComparer
{
    public record NormalizedRequest(
        string Path,
        [property: JsonIgnore] string Industry,
        [property: JsonIgnore] string Source,
        string Method,
        ImmutableArray<NormalizedProperty> QueryStringParamters,
        ImmutableArray<NormalizedProperty> RequestHeaders,
        ImmutableArray<NormalizedObject> RequestObjects,
        ImmutableArray<NormalizedResponse> Responses
        );
}
