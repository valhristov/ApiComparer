using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ApiComparer;
using OmsApiComparer.Swagger;

namespace OmsApiComparer
{
    public static class SwaggerAdapter
    {
        private static readonly Regex _requestPathExtractor = new("/api/v2/(?<industry>[^/]+)/(?<path>.*)");

        public static ImmutableArray<NormalizedRequest> Read(string swaggerDoc)
        {
            var document = JsonSerializer.Deserialize<ApiDefinition>(swaggerDoc);

            return ToApiRequests(document).ToImmutableArray();
        }

        private static IEnumerable<NormalizedRequest> ToApiRequests(ApiDefinition document)
        {
            foreach (var pathAndMethods in document.PathDefinitions)
            {
                var path = pathAndMethods.Key;

                var match = _requestPathExtractor.Match(path);
                var industry = match.Success ? match.Groups["industry"].Value : "unknown";
                var shortPath = match.Success ? match.Groups["path"].Value : "unknown";

                foreach (var methodAndRequest in pathAndMethods.Value)
                {
                    var method = methodAndRequest.Key;
                    var request = methodAndRequest.Value;

                    var apiRequest = new NormalizedRequest(
                        $"api/v2/{{industry}}/{shortPath}",
                        industry,
                        "Russia",
                        method,
                        GetQueryStringParameters(request),
                        GetRequestHeaders(request),
                        GetRequestObjects(request, GetSchema),
                        GetResponses(request, GetSchema));

                    yield return apiRequest;
                }
            }

            JsonSchema GetSchema(string name) =>
                document.Schemas.TryGetValue(name, out var schema) ? schema : null;
        }

        private static ImmutableArray<NormalizedObject> GetRequestObjects(RequestDefinition request, Func<string, JsonSchema> getSchema)
        {
            var bodyParameter = request.Parameters?.Where(p => p.Location == "body").SingleOrDefault();

            return bodyParameter == null
                ? ImmutableArray<NormalizedObject>.Empty
                : ToObjectDefinitionRecursive(getSchema(bodyParameter.Schema.Key), getSchema);
        }

        private static ImmutableArray<NormalizedObject> ToObjectDefinitionRecursive(JsonSchema schema, Func<string, JsonSchema> getSchema)
        {
            var result = ImmutableArray<NormalizedObject>.Empty;
            var schemasToProcess = new Queue<JsonSchema>();
            schemasToProcess.Enqueue(schema);

            while (schemasToProcess.Count > 0)
            {
                var currentSchema = schemasToProcess.Dequeue();
                var properties = ImmutableArray<NormalizedProperty>.Empty;
                foreach (var property in currentSchema.Properties)
                {
                    var propertyName = property.Key;
                    var definition = property.Value;
                    var propertyType = definition.Type;
                    switch (property.Value.Type)
                    {
                        case "array":
                            if (property.Value.ItemsType?.Key != null)
                            {
                                var nextSchema = getSchema(property.Value.ItemsType.Key);
                                schemasToProcess.Enqueue(nextSchema);
                                propertyType = nextSchema.Title + "[]";
                            }
                            break;
                        case "object":
                        case null:
                            if (property.Value.Reference != null)
                            {
                                var nextSchema = getSchema(property.Value.ReferenceKey);
                                schemasToProcess.Enqueue(nextSchema);
                                propertyType = nextSchema.Title;
                            }
                            break;
                        default: break; //do nothing
                    }
                    properties = properties.Add(new NormalizedProperty(propertyName, RemoveIndustry(propertyType), definition.Description, currentSchema.IsRequired(propertyName)));
                }
                result = result.Add(new NormalizedObject(RemoveIndustry(currentSchema.Title), currentSchema.Title, properties));
            }

            return result;
        }

        // Light and Fashion are the same thing
        private static readonly string[] knownIndustries = new[] {
            "beer", "bicycle", "light", "fashion", "lp", "milk", "ncp", "otp",
            "perfum", "pharma", "photo", "shoes", "tires", "tobacco",
            "water", "wheelchairs", };

        private static string RemoveIndustry(string title) =>
            knownIndustries.Aggregate(title,
                (currentTitle, industry) => currentTitle.Replace(industry, string.Empty, StringComparison.OrdinalIgnoreCase));

        private static ImmutableArray<NormalizedResponse> GetResponses(RequestDefinition request, Func<string, JsonSchema> getSchema)
        {
            return request.Responses == null
                ? ImmutableArray<NormalizedResponse>.Empty
                : request.Responses.Select(ToApiResponse).ToImmutableArray();

            NormalizedResponse ToApiResponse(KeyValuePair<string, ResponseReference> arg1)
            {
                var responseCode = arg1.Key;
                var responseDefinition = arg1.Value;

                return new NormalizedResponse(
                    int.Parse(responseCode),
                    ToObjectDefinitionRecursive(getSchema(responseDefinition.Schema.Key), getSchema));
            }
        }

        private static ImmutableArray<NormalizedProperty> GetRequestHeaders(RequestDefinition request)
        {
            return request.Parameters == null
                ? ImmutableArray<NormalizedProperty>.Empty
                : request.Parameters.Where(p => p.Location == "header").Select(ToRequestHeader).ToImmutableArray();

            static NormalizedProperty ToRequestHeader(ParameterDefinition parameter) =>
                new NormalizedProperty(parameter.Name, "string", parameter.Description, parameter.IsRequired);
        }

        private static ImmutableArray<NormalizedProperty> GetQueryStringParameters(RequestDefinition request)
        {
            return request.Parameters == null
                ? ImmutableArray<NormalizedProperty>.Empty
                : request.Parameters.Where(p => p.Location == "query").Select(ToQueryStringParameter).ToImmutableArray();

            static NormalizedProperty ToQueryStringParameter(ParameterDefinition parameter) =>
                new NormalizedProperty(parameter.Name, parameter.Type, parameter.Description, parameter.IsRequired);
        }
    }
}
