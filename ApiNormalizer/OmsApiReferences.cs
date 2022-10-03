using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ApiComparer;

namespace ApiNormalizer
{
    public class OmsApiReferences
    {
        private record Request(string Method, string Path)
        {
            public bool Matches(NormalizedRequest normalizedRequest) =>
                string.Equals(normalizedRequest.Method, Method, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(normalizedRequest.Path, Path, StringComparison.OrdinalIgnoreCase);
        }

        private static readonly ImmutableArray<Request> _requestsOfInterest = ImmutableArray.Create(
            new Request("POST", "api/v2/{industry}/orders"),
            new Request("POST", "api/v2/{industry}/buffer/close"),
            new Request("GET", "api/v2/{industry}/codes"),
            new Request("GET", "api/v2/{industry}/buffer/status"),
            new Request("GET", "api/v2/{industry}/ping"),
            new Request("GET", "api/v2/{industry}/report/info"),
            new Request("POST", "api/v2/{industry}/aggregation"),
            new Request("POST", "api/v2/{industry}/dropout"),
            new Request("POST", "api/v2/{industry}/utilisation")
            );

        private static readonly ImmutableArray<string> _industriesOfInterest = ImmutableArray.Create(
            "tobacco",
            "ncp",
            "otp",
            "beer",
            "water"
            );

        private readonly ImmutableArray<SwaggerDoc> _swaggerDocs;
        private ImmutableArray<NormalizedRequest> _normalizedRequests;

        public OmsApiReferences(ImmutableArray<SwaggerDoc> swaggerDocs)
        {
            _swaggerDocs = swaggerDocs;
        }

        public void Parse(Func<SwaggerDoc, ImmutableArray<NormalizedRequest>> parseSwaggerDoc)
        {
            _normalizedRequests = _swaggerDocs
                .Select(parseSwaggerDoc)
                .SelectMany(parsed => parsed)
                .Where(IsInteresting)
                .ToImmutableArray();

            static bool IsInteresting(NormalizedRequest request) =>
                _requestsOfInterest.Any(r => r.Matches(request)) &&
                _industriesOfInterest.Contains(request.Industry, StringComparer.OrdinalIgnoreCase);
        }

        public void Save(string targetDir)
        {
            // Write the raw JSONs
            foreach (var omsApiReference in _swaggerDocs)
            {
                Write(omsApiReference.Json, Path.Combine(targetDir, $"{omsApiReference.Country.ToLower()}.json"));
            }

            foreach (var request in _normalizedRequests)
            {
                var path = Path.Combine(targetDir, request.Source, $"{NormalizePath(request.Path.Replace("{industry}", request.Industry))} {request.Method.ToUpper()}.json");

                var content = JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true });

                Write(content, path);
            }

            static string NormalizePath(string path) =>
                Path.GetInvalidFileNameChars().Aggregate(path, (p, ch) => p.Replace(ch, '_'));
        }

        private static void Write(string content, string path)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (!string.IsNullOrWhiteSpace(content))
            {
                File.WriteAllText(path, content);
            }
        }
    }
}
