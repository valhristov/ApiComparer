using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    internal class ApiDefinition
    {
        [JsonPropertyName("tags")]
        public List<TagDefinition> Tags { get; set; }
        [JsonPropertyName("paths")]
        public Dictionary<string, Dictionary<string, RequestDefinition>> PathDefinitions { get; set; }
        [JsonPropertyName("definitions")]
        public Dictionary<string, JsonSchema> Schemas { get; set; }
    }
}
