using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiComparer.Swagger.Dtos
{
    internal class ApiDefinition
    {
        [JsonPropertyName("tags")]
        public List<TagDefinition> Tags { get; set; }
        [JsonPropertyName("paths")]
        public Dictionary<string, Dictionary<string, RequestDefinition>> PathDefinitions { get; set; }
        [JsonPropertyName("definitions")]
        public Dictionary<string, ObjectSchema> Schemas { get; set; }
    }
}
