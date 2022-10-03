using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiComparer.Swagger
{
    public static class SwaggerDownloader
    {
        public static async Task<string> Download(Uri omsBaseUrl)
        {
            try
            {
                var client = new HttpClient { BaseAddress = omsBaseUrl };
                var response = await client.GetStringAsync("v2/api-docs");
                return response;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
