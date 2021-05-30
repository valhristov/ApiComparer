using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    internal class TagDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}