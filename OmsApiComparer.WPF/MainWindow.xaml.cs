using System;
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
                    .Select(samePathRequests => new RequestComparisonViewModel(
                        samePathRequests.Key,
                        CreateComparisonViewModels(sources, samePathRequests, r => r.QueryStringParamters),
                        CreateComparisonViewModels(sources, samePathRequests, r => r.RequestHeaders)))
                    .ToImmutableArray());
        }

        private ImmutableArray<ComparisonViewModel<NormalizedProperty>> CreateComparisonViewModels(
            ImmutableArray<string> sources,
            IEnumerable<NormalizedRequest> samePathRequests,
            Func<NormalizedRequest, IEnumerable<NormalizedProperty>> getProperties)
        {
            var names = samePathRequests.SelectMany(getProperties).Select(p => p.Name).Distinct();

            var propertiesByIndustry = samePathRequests
                .SelectMany(r => getProperties(r).Select(p => (r.Industry, p)))
                .ToLookup(x => x.Industry, x => x.p);

            return names
                .Select(name =>
                    new ComparisonViewModel<NormalizedProperty>(
                        name,
                        GetSourceViewModel(sources.ElementAt(0), name),
                        GetSourceViewModel(sources.ElementAt(1), name),
                        GetSourceViewModel(sources.ElementAt(2), name),
                        GetSourceViewModel(sources.ElementAt(3), name)
                    ))
                .ToImmutableArray();

            SourceViewModel<NormalizedProperty> GetSourceViewModel(string source, string name) =>
                new SourceViewModel<NormalizedProperty>(
                    source,
                    propertiesByIndustry[source].FirstOrDefault(p => p.Name == name));
        }
    }
}
