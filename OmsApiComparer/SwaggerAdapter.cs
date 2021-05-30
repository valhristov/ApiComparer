using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using OmsApiComparer.Swagger;

namespace OmsApiComparer
{
    public static class SwaggerAdapter
    {
        public static ImmutableArray<ApiRequest> Read(string swaggerDoc)
        {
            var document = JsonSerializer.Deserialize<ApiDefinition>(swaggerDoc);

            return ToApiRequests(document).ToImmutableArray();
        }

        private static IEnumerable<ApiRequest> ToApiRequests(ApiDefinition document)
        {
            foreach (var pathAndMethods in document.PathDefinitions)
            {
                var path = pathAndMethods.Key;

                foreach (var methodAndRequest in pathAndMethods.Value)
                {
                    var method = methodAndRequest.Key;
                    var request = methodAndRequest.Value;

                    var apiRequest = new ApiRequest(
                        path,
                        method,
                        request.Summary,
                        GetQueryStringParameters(request),
                        GetRequestHeaders(request),
                        GetRequestBody(request, GetSchema),
                        GetResponses(request, GetSchema));

                    yield return apiRequest;
                }
            }

            JsonSchema GetSchema(string name) =>
                document.Schemas.TryGetValue(name, out var schema) ? schema : null;
        }


        private static RequestBody GetRequestBody(RequestDefinition request, Func<string, JsonSchema> getSchema)
        {
            return request.Parameters == null
                ? null
                : request.Parameters.Where(p => p.Location == "body").Select(ToRequestBody).FirstOrDefault();

            RequestBody ToRequestBody(ParameterDefinition parameter)
            {
                var schema = getSchema(parameter.Schema.Key);

                return new RequestBody
                {
                    Properties = GetProperties(schema),
                };
            }
        }

        private static ImmutableArray<BodyProperty> GetProperties(JsonSchema schema)
        {
            return schema == null
                ? ImmutableArray<BodyProperty>.Empty
                : schema.Properties.Select(ToBodyProperty).ToImmutableArray();

            BodyProperty ToBodyProperty(KeyValuePair<string, PropertySchema> nameAndDefinition)
            {
                var propertyName = nameAndDefinition.Key;
                var definition = nameAndDefinition.Value;
                return new BodyProperty
                {
                    Name = propertyName,
                    Description = definition.Description,
                    IsRequired = schema.RequiredProperties.Contains(propertyName),
                    Type = definition.Type,
                };
            }
        }

        private static ImmutableArray<ApiResponse> GetResponses(RequestDefinition request, Func<string, JsonSchema> getSchema)
        {
            return request.Responses == null
                ? ImmutableArray<ApiResponse>.Empty
                : request.Responses.Select(ToApiResponse).ToImmutableArray();

            static ApiResponse ToApiResponse(KeyValuePair<string, ResponseReference> arg1)
            {
                var responseCode = arg1.Key;
                var responseDefinition = arg1.Value;
                return new ApiResponse
                {
                    StatusCode = responseCode,
                };
            }
        }

        private static ImmutableArray<RequestHeader> GetRequestHeaders(RequestDefinition request)
        {
            return request.Parameters == null
                ? ImmutableArray<RequestHeader>.Empty
                : request.Parameters.Where(p => p.Location == "header").Select(ToRequestHeader).ToImmutableArray();

            static RequestHeader ToRequestHeader(ParameterDefinition parameter) =>
                new RequestHeader
                {
                    Name = parameter.Name,
                    Description = parameter.Description,
                    IsRequired = parameter.IsRequired,
                };
        }

        private static ImmutableArray<QueryStringParameter> GetQueryStringParameters(RequestDefinition request)
        {
            return request.Parameters == null
                ? ImmutableArray<QueryStringParameter>.Empty
                : request.Parameters.Where(p => p.Location == "query").Select(ToQueryStringParameter).ToImmutableArray();

            static QueryStringParameter ToQueryStringParameter(ParameterDefinition parameter) =>
                new QueryStringParameter
                {
                    Name = parameter.Name,
                    Description = parameter.Description,
                    Type = parameter.Type,
                    IsRequired = parameter.IsRequired,
                };
        }
    }
}
