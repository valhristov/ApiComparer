using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OmsApiComparer;

namespace ApiNormalizer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var requestsRU = await SwaggerAdapter.Read(new Uri("https://intuot.crpt.ru:12011"), "RU");
            var requestsKZ = await SwaggerAdapter.Read(new Uri("https://suzcloud.stage.ismet.kz"), "KZ");
            var requestsKG = await SwaggerAdapter.Read(new Uri("https://oms.megacom.kg/v2/api-docs"), "KG");

            foreach (var r in requestsRU.Union(requestsKZ).Union(requestsKG))
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
