using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    public class ParameterDefinition
    {
        [JsonPropertyName("in")]
        public string Location { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("required")]
        public bool IsRequired { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("schema")]
        public SchemaReference Schema { get; set; }
    }
}
