using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    public class TagDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}