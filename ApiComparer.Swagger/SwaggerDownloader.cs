using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiComparer.Swagger
{
    public static class SwaggerDownloader
    {
        private static readonly Regex _apiExamplesFixer = new("(\"example\"\\:\\s*?\\[)(?<wrong>['\\w,\\s]+)+(\\])");

        public static async Task<string> Download(Uri omsBaseUrl)
        {
            try
            {
                var client = new HttpClient { BaseAddress = omsBaseUrl };

                var response = await client.GetStringAsync("v2/api-docs");

                return Fix(response);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string Fix(string json)
        {
            // Some field examples have invalid JSON, this removes them
            return _apiExamplesFixer.Replace(json, "\"example\":[]");
        }
    }
}
