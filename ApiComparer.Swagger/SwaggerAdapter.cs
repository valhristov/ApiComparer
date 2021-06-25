using ApiComparer.Swagger.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ApiComparer.Swagger
{
    public static class SwaggerAdapter
    {
        private static readonly Regex _requestPathExtractor = new("/api/v2/(?<industry>[^/]+)/(?<path>.*)");

        public static ImmutableArray<NormalizedRequest> Parse(string api, string source)
        {
            try
            {
                var document = JsonSerializer.Deserialize<ApiDefinition>(api);

                return ToApiRequests(document, source).ToImmutableArray();
            }
            catch
            {
                return ImmutableArray<NormalizedRequest>.Empty;
            }
        }

        private static IEnumerable<NormalizedRequest> ToApiRequests(ApiDefinition document, string source)
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
                        source,
                        method,
                        GetQueryStringParameters(request),
                        GetRequestHeaders(request),
                        GetRequestObjects(request, GetSchema),
                        GetResponses(request, GetSchema));

                    yield return apiRequest;
                }
            }

            ObjectSchema GetSchema(string name) =>
                document.Schemas.TryGetValue(name, out var schema) ? schema : null;
        }

        private static ImmutableArray<NormalizedObject> GetRequestObjects(RequestDefinition request, Func<string, ObjectSchema> getSchema)
        {
            var bodyParameter = request.Parameters?.Where(p => p.Location == "body").SingleOrDefault();

            return bodyParameter == null
                ? ImmutableArray<NormalizedObject>.Empty
                : ToObjectDefinitionRecursive(getSchema(bodyParameter.Schema.Key), getSchema);
        }

        private static ImmutableArray<NormalizedObject> ToObjectDefinitionRecursive(ObjectSchema schema, Func<string, ObjectSchema> getSchema)
        {
            var result = ImmutableArray<NormalizedObject>.Empty;
            var schemasToProcess = new Queue<ObjectSchema>();
            schemasToProcess.Enqueue(schema);

            while (schemasToProcess.Count > 0)
            {
                var currentSchema = schemasToProcess.Dequeue();
                var properties = ImmutableArray<NormalizedProperty>.Empty;
                foreach (var property in currentSchema.Properties)
                {
                    var propertyName = property.Key;
                    var propertySchema = property.Value;
                    var propertyType = propertySchema.Type;
                    switch (propertyType)
                    {
                        case "array":
                            if (propertySchema.ItemsType?.Key != null)
                            {
                                var nextSchema = getSchema(propertySchema.ItemsType.Key);
                                schemasToProcess.Enqueue(nextSchema);
                                propertyType = nextSchema.Title + "[]";
                            }
                            break;
                        case "object":
                        case null:
                            if (propertySchema.Reference != null)
                            {
                                var nextSchema = getSchema(propertySchema.ReferenceKey);
                                schemasToProcess.Enqueue(nextSchema);
                                propertyType = nextSchema.Title;
                            }
                            break;
                        case "string":
                            if (propertySchema.Values != null)
                            {
                                result = result.Add(new NormalizedObject(propertyName, propertyName, propertySchema.Values.Select(v => new NormalizedProperty(v.ToString(), propertyType, string.Empty, false)).ToImmutableArray()));
                                propertyType = propertyName;
                            }
                            break;
                        case "integer":
                            if (propertySchema.Values != null)
                            {
                                result = result.Add(new NormalizedObject(propertyName, propertyName, propertySchema.Values.Select(v => new NormalizedProperty(v.ToString(), propertyType, string.Empty, false)).ToImmutableArray()));
                                propertyType = propertyName;
                            }
                            break;
                        default: break; //do nothing
                    }
                    properties = properties.Add(new NormalizedProperty(propertyName, RemoveIndustry(propertyType), propertySchema.Description, currentSchema.IsRequired(propertyName)));
                }
                result = result.Add(new NormalizedObject(RemoveIndustry(currentSchema.Title), currentSchema.Title, properties));
            }

            return result;
        }

        private static ObjectSchema CreateEnum<T>(string name, T[] values) =>
            new ObjectSchema
            {
                Title = UpperCaseFirstLetter(name),
                Properties = values
                    .Where(v => v != null)
                    .Select(v => (Name: v.ToString(), Property: new PropertySchema { Type = "enum", }))
                    .ToDictionary(x => x.Name, x => x.Property),
            };

        public static string UpperCaseFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            return string.Create(s.Length, s, (chars, state) =>
            {
                state.AsSpan().CopyTo(chars);
                chars[0] = char.ToUpper(chars[0]);
            });
        }

        private static readonly string[] knownIndustries = new[] {
            // Light and Fashion are the same thing
            "beer", "bicycle", "light", "fashion", "lp", "milk", "ncp", "otp",
            "perfum", "pharma", "photo", "shoes", "tires", "tobacco",
            "water", "wheelchairs",
            // Adding KG and KZ here
            "kg", "kz"};

        private static string RemoveIndustry(string title) =>
            knownIndustries.Aggregate(title,
                (currentTitle, industry) => currentTitle.Replace(industry, string.Empty, StringComparison.OrdinalIgnoreCase));

        private static ImmutableArray<NormalizedResponse> GetResponses(RequestDefinition request, Func<string, ObjectSchema> getSchema)
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
