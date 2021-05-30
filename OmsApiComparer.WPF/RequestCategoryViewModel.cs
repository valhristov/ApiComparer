using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace OmsApiComparer.WPF
{
    public class RequestCategoryViewModel
    {
        public string MethodAndPath { get; }

        public ImmutableArray<MultiIndustryValue<ObjectProperty>> QueryStringParameters { get; }
        public ImmutableArray<MultiIndustryValue<RequestHeader>> RequestHeaders { get; }
        public ImmutableArray<MultiIndustryValue<ObjectProperty>> RequestBodyProperties { get; }

        public RequestCategoryViewModel(IEnumerable<ApiRequest> requests)
        {
            var first = requests.First();

            var requestsByIndustry = requests.ToLookup(r => r.Industry, r => r);

            // all requests are supposed to have the same path and method
            MethodAndPath = $"{first.Method} {first.FullPath}";

            var queryStringParametersByNameAndIndustry = requests
                .SelectMany(r => r.QueryStringParameters.Select(p => (Industry: r.Industry, Parameter: p)))
                .GroupBy(x => x.Parameter.Name, x => x);

            QueryStringParameters = queryStringParametersByNameAndIndustry
                .Select(g => new MultiIndustryValue<ObjectProperty>(g))
                .ToImmutableArray();

            var requestHeadersByNameAndIndustry = requests
                .SelectMany(r => r.RequestHeaders.Select(p => (Industry: r.Industry, Parameter: p)))
                .GroupBy(x => x.Parameter.Name, x => x);

            RequestHeaders = requestHeadersByNameAndIndustry
                .Select(g => new MultiIndustryValue<RequestHeader>(g))
                .ToImmutableArray();

            var requestBodyPropertiesByNameAndIndustry = requests
                .SelectMany(r => r.RequestBody.Properties.Select(p => (Industry: r.Industry, Parameter: p)))
                .GroupBy(x => x.Parameter.Name, x => x);

            RequestBodyProperties = requestBodyPropertiesByNameAndIndustry
                .Select(g => new MultiIndustryValue<ObjectProperty>(g))
                .ToImmutableArray();
        }

        public override string ToString() => $"{MethodAndPath}";
    }
}
