using System.Collections.Immutable;

namespace OmsApiComparer.WPF
{
    public record RequestViewModel(
        string MethodAndPath,
        ImmutableArray<PropertyViewModel> QueryString,
        ImmutableArray<PropertyViewModel> Headers,
        ImmutableArray<ObjectViewModel> RequestObjects,
        ImmutableArray<ResponseViewModel> Responses,
        ImmutableArray<ObjectViewModel> ResponseObjects
        );
}
