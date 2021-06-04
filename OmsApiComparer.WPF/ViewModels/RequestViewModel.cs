using System.Collections.Immutable;

namespace OmsApiComparer.WPF
{
    public record RequestViewModel(
        string MethodAndPath,
        ObjectViewModel QueryString,
        ObjectViewModel RequestHeaders,
        ImmutableArray<ObjectViewModel> RequestObjects,
        ImmutableArray<ResponseViewModel> Responses,
        ImmutableArray<ObjectViewModel> ResponseObjects
        );
}
