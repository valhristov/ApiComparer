using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ApiComparer;

namespace OmsApiComparer.WPF
{
    public partial class MainWindow : Window
    {
        private static readonly ImmutableArray<string> _sourcesOfInterest =
            ImmutableArray.Create("tobacco", "ncp", "otp", "milk");

        public MainWindow()
        {
            InitializeComponent();

            CreateContext();
        }

        private async void CreateContext()
        {
            var requests = await SwaggerAdapter.Read(new Uri("https://intuot.crpt.ru:12011"));

            //var requests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

            var requestsByPath = requests.GroupBy(x => $"{x.Path} {x.Method}");

            DataContext = new MainWindowViewModel(
                requestsByPath
                    .Select(samePathRequests => CreateRequestViewModel(samePathRequests.Key, samePathRequests))
                    .ToImmutableArray());

            RequestViewModel CreateRequestViewModel(string pathAndMethod, IEnumerable<NormalizedRequest> samePathRequests) =>
                new RequestViewModel(
                    pathAndMethod,
                    CreatePropertyViewModels(samePathRequests, r => r.QueryStringParamters),
                    CreatePropertyViewModels(samePathRequests, r => r.RequestHeaders),
                    CreateObjectViewModels(samePathRequests),
                    CreateResponseObjectViewModels(samePathRequests));
        }

        private ImmutableArray<ObjectViewModel> CreateResponseObjectViewModels(IEnumerable<NormalizedRequest> samePathRequests)
        {
            var objectViewModels = samePathRequests
                .SelectMany(r => r.Responses.SelectMany(res => res.ResponseObjects))
                .Select(o => o.Name)
                .Distinct()
                .Select(CreateObjectViewModel)
                .ToImmutableArray();

            return objectViewModels;

            ObjectViewModel CreateObjectViewModel(string objectName) =>
                new ObjectViewModel(objectName,
                    CreatePropertyViewModels(samePathRequests, r => r.Responses.SelectMany(res => res.ResponseObjects).Where(o => o.Name == objectName).SelectMany(o => o.Properties)));
        }

        private ImmutableArray<ObjectViewModel> CreateObjectViewModels(
            IEnumerable<NormalizedRequest> samePathRequests)
        {
            var objectViewModels = samePathRequests
                .SelectMany(r => r.RequestObjects)
                .Select(o => o.Name)
                .Distinct()
                .Select(CreateObjectViewModel)
                .ToImmutableArray();

            return objectViewModels;

            ObjectViewModel CreateObjectViewModel(string objectName) =>
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
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest[0], name),
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest[1], name),
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest[2], name),
                        CreatePropertyWithSourceViewModel(_sourcesOfInterest[3], name)
                    ))
                .ToImmutableArray();

            return viewModels;

            PropertyWithSourceViewModel CreatePropertyWithSourceViewModel(string source, string name) =>
                new PropertyWithSourceViewModel(
                    source,
                    name,
                    propertiesByIndustry[source].FirstOrDefault(p => p.Name == name));
        }
    }
}
