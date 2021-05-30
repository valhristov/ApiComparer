using System.Collections.Immutable;

namespace ApiComparer
{
    public record NormalizedResponse(
        int StatusCode,
        ImmutableArray<NormalizedObject> ResponseObjects
        );
}
