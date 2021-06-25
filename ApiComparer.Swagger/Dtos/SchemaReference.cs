using System.Text.Json.Serialization;

namespace ApiComparer.Swagger.Dtos
{
    internal class SchemaReference
    {
        [JsonPropertyName("$ref")]
        public string ReferenceName { get; set; }

        public string Key => ReferenceName == null ? null : ReferenceName.Substring(ReferenceName.LastIndexOf('/') + 1);
    }
}
