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
        private static readonly ImmutableHashSet<string> _sourcesOfInterest =
            ImmutableHashSet.Create("tobacco", "ncp", "otp", "milk");

        public MainWindow()
        {
            InitializeComponent();

            var requests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

            var requestsByPath = requests.GroupBy(x => $"{x.Path} {x.Method}");

            DataContext = new MainWindowViewModel(
                requestsByPath
                    .Select(samePathRequests =>
                        new RequestViewModel(
                            samePathRequests.Key,
                            CreatePropertyViewModels(samePathRequests, r => r.QueryStringParamters),
                            CreatePropertyViewModels(samePathRequests, r => r.RequestHeaders),
                            CreateObjectViewModels(samePathRequests)))
                    .ToImmutableArray());
        }

        private ImmutableArray<ObjectViewModel> CreateObjectViewModels(
            IEnumerable<NormalizedRequest> samePathRequests)
        {
            var objectViewModels = samePathRequests
                .SelectMany(r => r.RequestObjects)
                .Select(o => o.Name)
                .Distinct()
                .Select(CreateRequestObjectViewModel)
                .ToImmutableArray();

            return objectViewModels;

            ObjectViewModel CreateRequestObjectViewModel(string objectName) =>
                new ObjectViewModel(objectName,
                    CreatePropertyViewModels(samePathRequests,
                        r => r.RequestObjects.Where(o => o.Name == objectName).SelectMany(o => o.Properties)));
        }

        private ImmutableArray<PropertyViewModel> CreatePropertyViewModels(
            IEnumerable<NormalizedRequest> samePathRequests,
            Func<NormalizedRequest, IEnumerable<NormalizedProperty>> getProperties)
        {
            var propertyNames = samePathRequests.SelectMany(getProperties).Select(p => p.Name).Distinct();

            var propertiesByIndustry = samePathRequests
                .SelectMany(x => getProperties(x).Select(p => (x.Industry, p)))
                .ToLookup(x => x.Industry, x => x.p);

            var viewModels = propertyNames
                .Select(name =>
                    new PropertyViewModel(
                        name,
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest.ElementAt(0), name),
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest.ElementAt(1), name),
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest.ElementAt(2), name),
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest.ElementAt(3), name)
                    ))
                .ToImmutableArray();
            return viewModels;

            PropertyWithSourceViewModel CreatePropertyWithSourceViewModel(string source, string name) =>
                new PropertyWithSourceViewModel(
                    source,
                    propertiesByIndustry[source].FirstOrDefault(p => p.Name == name));
        }

    }
}
