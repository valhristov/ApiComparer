using ApiComparer.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiNormalizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ////var requestsRU = SwaggerAdapter.Parse(await SwaggerDownloader.Download(new Uri("https://suz.sandbox.crpt.tech")), "RU");
            ////var requestsKZ = SwaggerAdapter.Parse(await SwaggerDownloader.Download(new Uri("https://suzcloud.stage.ismet.kz")), "KZ");
            ////var requestsKG = SwaggerAdapter.Parse(await SwaggerDownloader.Download(new Uri("https://oms.megacom.kg")), "KG");
            ////var requestsKG = SwaggerAdapter.Parse(await SwaggerDownloader.Download(new Uri("https://oms.megacom.kg")), "KG");
            var requestsRU = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\ru.json"), "RU");
            var requestsKZ = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\kz.json"), "KZ");
            var requestsKG = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\kg.json"), "KG");
            var requestsUZ = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\uz.json"), "UZ");

            foreach (var r in requestsRU.Union(requestsKZ).Union(requestsKG).Union(requestsUZ))
            {
                var path = $"c:\\work\\{DateTime.Now:yyMMddHHmm}\\{r.Industry} {r.Source}\\{NormalizePath(r.Path)} {r.Method.ToUpper()}.json";
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, JsonSerializer.Serialize(r, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        private static string NormalizePath(string path) =>
            Path.GetInvalidFileNameChars().Aggregate(path, (p, ch) => p.Replace(ch, '_'));
    }
}
