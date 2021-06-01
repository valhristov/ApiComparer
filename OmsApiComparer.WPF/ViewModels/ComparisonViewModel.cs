namespace OmsApiComparer.WPF
{
    public record ComparisonViewModel(
        string Name ,
        SourceValueViewModel A,
        SourceValueViewModel B,
        SourceValueViewModel C,
        SourceValueViewModel D
        );

}
