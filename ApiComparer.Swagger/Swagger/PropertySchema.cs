using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    internal class PropertySchema
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

        public string ReferenceKey => Reference == null ? null : Reference.Substring(Reference.LastIndexOf('/') + 1);
    }
}
