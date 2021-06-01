namespace OmsApiComparer.WPF
{
    public record PropertyViewModel(
        string Name ,
        PropertyWithSourceViewModel A,
        PropertyWithSourceViewModel B,
        PropertyWithSourceViewModel C,
        PropertyWithSourceViewModel D
        );

}
