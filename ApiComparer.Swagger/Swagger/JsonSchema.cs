using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    internal class JsonSchema
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("required")]
        public List<string> RequiredProperties { get; set; }

        [JsonPropertyName("properties")]
        public Dictionary<string, PropertySchema> Properties { get; set; }

        public bool IsRequired(string propertyName) =>
            RequiredProperties != null && RequiredProperties.Contains(propertyName);
    }
}
