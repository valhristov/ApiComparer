using System.Text.Json.Serialization;

namespace ApiComparer.Swagger.Dtos
{
    internal class TagDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}