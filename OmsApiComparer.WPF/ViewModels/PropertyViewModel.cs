namespace OmsApiComparer.WPF
{
    public record PropertyViewModel(
        string Name,
        PropertyWithSourceViewModel TobaccoRU,
        PropertyWithSourceViewModel NcpRU,
        PropertyWithSourceViewModel OtpRU,
        PropertyWithSourceViewModel MilkRU,
        PropertyWithSourceViewModel WaterRU,
        PropertyWithSourceViewModel BeerRU,
        PropertyWithSourceViewModel TobaccoKZ,
        PropertyWithSourceViewModel TobaccoKG,
        PropertyWithSourceViewModel TobaccoUZ,
        PropertyWithSourceViewModel BeerUZ,
        PropertyWithSourceViewModel WaterUZ
        );
}
