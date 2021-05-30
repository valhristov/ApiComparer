using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    public class SchemaReference
    {
        [JsonPropertyName("$ref")]
        public string ReferenceName { get; set; }

        public string Key => ReferenceName.Substring(ReferenceName.LastIndexOf('/'));
    }
}
