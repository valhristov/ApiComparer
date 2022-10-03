using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ApiComparer.Swagger
{
    public static class SwaggerFixer
    {
        private static readonly Regex _apiExamplesFixer = new("(\"example\"\\:\\s*?\\[)(?<wrong>['\\w,\\s]+)+(\\])");
        private static readonly Regex _apiOperationIdFixer = new("(\"operationId\"\\:\\s*?\"\\w+?)(_\\d+?)(\",)");

        public static string Fix(string json)
        {
            // Some field examples have invalid JSON, this removes it and fixes the operation IDs
            var replaced = _apiExamplesFixer.Replace(_apiOperationIdFixer.Replace(json, "$1$3"), "\"example\":[]");
            // Indent json string, so that it could be saved later
            var indented = JsonNode.Parse(replaced).ToJsonString(new JsonSerializerOptions { WriteIndented = true, });
            return indented;
        }
    }
}
