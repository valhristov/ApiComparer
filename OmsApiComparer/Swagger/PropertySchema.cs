using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    public class PropertySchema
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("example")]
        public object Example { get; set; }

        [JsonPropertyName("$ref")]
        public string Reference { get; set; }

        [JsonPropertyName("items")]
        public SchemaReference ItemsType { get; set; }
    }
}
