using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiComparer.Swagger.Dtos
{
    internal class RequestDefinition
    {
        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("operationId")]
        public string OperationId { get; set; }

        [JsonPropertyName("parameters")]
        public List<ParameterDefinition> Parameters { get; set; }

        [JsonPropertyName("responses")]
        public Dictionary<string, ResponseReference> Responses { get; set; }

        [JsonPropertyName("deprecated")]
        public bool Deprecated { get; set; }
    }
}
