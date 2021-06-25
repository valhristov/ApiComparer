using System.IO;
using System.Threading.Tasks;

namespace ApiComparer.Swagger
{
    public static class SwaggerFileReader
    {
        public static async Task<string> Read(string path) =>
            await File.ReadAllTextAsync(path);
    }
}
