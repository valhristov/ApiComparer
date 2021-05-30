using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace OmsApiComparer
{
    public class ApiRequest
    {
        private static readonly Regex _extractor = new("/api/v2/(?<industry>[^/]+)/(?<path>.+)");

        public string FullPath { get; }
        public string Path { get; }
        public string Industry { get; }
        public string Method { get; }
        public string Summary { get; }
        public ImmutableArray<QueryStringParameter> QueryStringParameters { get; }
        public ImmutableArray<RequestHeader> RequestHeaders { get; }
        public RequestBody RequestBody { get; }
        public ImmutableArray<ApiResponse> Responses { get; }

        public ApiRequest(string fullPath, string method, string summary, ImmutableArray<QueryStringParameter> queryStringParameters, ImmutableArray<RequestHeader> requestHeaders, RequestBody requestBody, ImmutableArray<ApiResponse> responses)
        {
            FullPath = fullPath;

            Responses = responses;
            RequestBody = requestBody;
            RequestHeaders = requestHeaders;
            QueryStringParameters = queryStringParameters;
            Summary = summary;
            Method = method;

            var match = _extractor.Match(fullPath);
            if (match.Success)
            {
                Industry = match.Groups["industry"].Value;
                Path = match.Groups["path"].Value;
            }
        }
    }

    public class QueryStringParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
    }

    public class RequestHeader
    {
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
    }

    public class RequestBody
    {
        public ImmutableArray<BodyProperty> Properties { get; set; }
    }

    public class BodyProperty
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsRequired { get; set; }
        public string Description { get; set; }
    }

    public class ApiResponse
    {
        public string StatusCode { get; set; }
    }

}
