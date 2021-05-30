using System.Text.Json.Serialization;

namespace OmsApiComparer.Swagger
{
    public class ResponseReference
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("schema")]
        public SchemaReference Schema { get; set; }
    }
}
