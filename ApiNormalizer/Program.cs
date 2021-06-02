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
            var requests = await SwaggerAdapter.Read(new Uri("https://intuot.crpt.ru:12011"));
            //var requests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

            foreach (var r in requests)
            {
                var path = $"c:\\work\\{DateTime.Now:yyMMddHHmm}\\{r.Industry}\\{NormalizePath(r.Path)} {r.Method.ToUpper()}.json";
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, JsonSerializer.Serialize(r, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        private static string NormalizePath(string path) =>
            Path.GetInvalidFileNameChars().Aggregate(path, (p, ch) => p.Replace(ch, '_'));
    }
}
