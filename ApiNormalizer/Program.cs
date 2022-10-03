using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiComparer.Swagger;

namespace ApiNormalizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var targetDir = Path.GetFullPath(args.ElementAtOrDefault(0) ?? Directory.GetCurrentDirectory());
            var sourceDir = args.ElementAtOrDefault(1);

            Func<Country, Task<string>> apiReader = sourceDir != null
                ? async country => SwaggerFixer.Fix(await SwaggerFileReader.Read(Path.Combine(sourceDir, $"{country.Name}.json")))
                : async country => SwaggerFixer.Fix(await SwaggerDownloader.Download(country.OmsUrl));

            var omsApiReferences = await SupportedCountries.GetOmsApis(apiReader);

            omsApiReferences.Parse(apiReference => SwaggerAdapter.Parse(apiReference.Json, apiReference.Country));

            omsApiReferences.Save(targetDir);
        }
    }
}
