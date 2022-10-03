using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace ApiNormalizer
{
    public record Country(string Name, Uri OmsUrl);

    public static class SupportedCountries
    {
        private static readonly ImmutableArray<Country> _supportedCountries = ImmutableArray.Create(
            new Country("ru", new Uri("https://suz.sandbox.crptech.ru")),
            new Country("kg", new Uri("https://oms.megacom.kg")),
            new Country("kz", new Uri("https://suzcloud.stage.ismet.kz")),
            new Country("uz", new Uri("https://omscloud.asllikbelgisi.uz"))
            );

        public static async Task<OmsApiReferences> GetOmsApis(Func<Country, Task<string>> download)
        {
            var tasks = _supportedCountries.Select(async country => await download(country)).ToArray();

            var jsons = await Task.WhenAll(tasks);

            var omsApiReferences = _supportedCountries
                .Zip(jsons, (country, json) => new SwaggerDoc(country.Name, json))
                .ToImmutableArray();

            return new OmsApiReferences(omsApiReferences);
        }
    }
}
