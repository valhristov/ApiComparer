using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Windows;
using ApiComparer;

namespace OmsApiComparer.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var requests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

            var sources = requests.Select(x => x.Industry).Distinct().ToImmutableArray();

            var requestsByPath = requests.GroupBy(x => $"{x.Path} {x.Method}");

            DataContext = new MainWindowViewModel(
                requestsByPath
                    .Select(samePathRequests =>
                    {
                        var xxx = CreateComparisonViewModels(sources, samePathRequests.Select(r => (r.Industry, r.QueryStringParamters)));
                        var yyy = CreateComparisonViewModels(sources, samePathRequests.Select(r => (r.Industry, r.RequestHeaders)));

                        var requestObjectNames = samePathRequests.SelectMany(r => r.RequestObjects).Select(o => o.Name).Distinct();

                        var zzz = requestObjectNames
                            .Select(name => new RequestObjectViewModel(name,
                                CreateComparisonViewModels(
                                    sources,
                                    // Take all properties for the object with name name, from all requests
                                    samePathRequests.Select(r =>
                                        (r.Industry,
                                         r.RequestObjects
                                            .Where(o => o.Name == name)
                                            .Select(o => o.Properties)
                                            .Union(new[] { ImmutableArray<NormalizedProperty>.Empty })
                                            .FirstOrDefault())).ToImmutableArray())))
                            .ToImmutableArray();

                        return new RequestComparisonViewModel(
                            samePathRequests.Key,
                            xxx,
                            yyy,
                            zzz);
                    })
                    .ToImmutableArray());
        }

        private ImmutableArray<ComparisonViewModel> CreateComparisonViewModels(
            ImmutableArray<string> sources,
            IEnumerable<(string Industry, ImmutableArray<NormalizedProperty> Properties)> propertyContainers)
        {
            var propertyNames = propertyContainers.SelectMany(x => x.Properties).Select(p => p.Name).Distinct();

            var propertiesByIndustry = propertyContainers
                .SelectMany(x => x.Properties.Select(p => (x.Industry, p)))
                .ToLookup(x => x.Industry, x => x.p);

            return propertyNames
                .Select(name =>
                    new ComparisonViewModel(
                        name,
                        GetSourceViewModel(sources.ElementAt(0), name),
                        GetSourceViewModel(sources.ElementAt(1), name),
                        GetSourceViewModel(sources.ElementAt(2), name),
                        GetSourceViewModel(sources.ElementAt(3), name)
                    ))
                .ToImmutableArray();

            SourceValueViewModel GetSourceViewModel(string source, string name) =>
                new SourceValueViewModel(
                    source,
                    propertiesByIndustry[source].FirstOrDefault(p => p.Name == name));
        }

    }
}
