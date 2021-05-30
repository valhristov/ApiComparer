using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    public class JsonSchema
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("required")]
        public List<string> RequiredProperties { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, PropertySchema> Properties { get; set; }
    }
}
