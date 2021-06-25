using System.Text.Json.Serialization;

namespace ApiComparer.Swagger.Dtos
{
    internal class ResponseReference
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("schema")]
        public SchemaReference Schema { get; set; }
    }
}
