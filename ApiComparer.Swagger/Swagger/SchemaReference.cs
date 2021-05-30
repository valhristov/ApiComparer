using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    internal class SchemaReference
    {
        [JsonPropertyName("$ref")]
        public string ReferenceName { get; set; }

        public string Key => ReferenceName == null ? null : ReferenceName.Substring(ReferenceName.LastIndexOf('/') + 1);
    }
}
