using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using OmsApiComparer;

namespace ApiNormalizer
{
    class Program
    {
        static void Main(string[] args)
        {
            var requests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

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
