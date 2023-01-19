using ApiComparer;
using ApiComparer.Swagger;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;

namespace OmsApiComparer.WPF
{
    public partial class MainWindow : Window
    {
        private static readonly ImmutableArray<string> _sourcesOfInterest =
            ImmutableArray.Create("tobacco", "ncp", "otp", "milk", "water", "beer");

        public MainWindow()
        {
            InitializeComponent();

            CreateContext();
        }

        private async void CreateContext()
        {
            //var requestsRU = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\ru.json"), "RU");
            //var requestsKZ = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\kz.json"), "KZ");
            //var requestsKG = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\kg.json"), "KG");
            //var requestsUZ = SwaggerAdapter.Parse(await SwaggerFileReader.Read("C:\\Work\\OmsApi\\uz.json"), "UZ");
            var requestsRU = SwaggerAdapter.Parse(SwaggerFixer.Fix(await SwaggerDownloader.Download(new Uri("https://suz.sandbox.crptech.ru"))), "RU");
            var requestsKZ = SwaggerAdapter.Parse(SwaggerFixer.Fix(await SwaggerDownloader.Download(new Uri("https://suzcloud.stage.ismet.kz"))), "KZ");
            var requestsKG = SwaggerAdapter.Parse(SwaggerFixer.Fix(await SwaggerDownloader.Download(new Uri("https://oms.megacom.kg"))), "KG");
            var requestsUZ = SwaggerAdapter.Parse(SwaggerFixer.Fix(await SwaggerDownloader.Download(new Uri("https://omscloud.asllikbelgisi.uz"))), "UZ");

            var requests = requestsRU.Union(requestsKZ).Union(requestsKG).Union(requestsUZ)
                .Where(r => _sourcesOfInterest.Contains(r.Industry)).ToList();

            var requestsByPath = requests.GroupBy(x => $"{x.Path} {x.Method}");

            DataContext = new MainWindowViewModel(
                requestsByPath
                    .Select(samePathRequests => CreateRequestViewModel(samePathRequests.Key, samePathRequests))
                    .ToImmutableArray());

            RequestViewModel CreateRequestViewModel(string pathAndMethod, IEnumerable<NormalizedRequest> samePathRequests) =>
                new RequestViewModel(
                    pathAndMethod,
                    new ObjectViewModel("Query string", CreatePropertyViewModels(samePathRequests, r => r.QueryStringParamters)),
                    new ObjectViewModel("Request headers", CreatePropertyViewModels(samePathRequests, r => r.RequestHeaders)),
                    CreateObjectViewModels(samePathRequests),
                    CreateResponseModels(samePathRequests),
                    CreateResponseObjectViewModels(samePathRequests));
        }

        private ImmutableArray<ResponseViewModel> CreateResponseModels(IEnumerable<NormalizedRequest> samePathRequests)
        {
            var responseViewModels = samePathRequests
                .SelectMany(r => r.Responses.Select(res => res.StatusCode))
                .Distinct()
                .Select(CreateResponseViewModel)
                .ToImmutableArray();

            return responseViewModels;

            ResponseViewModel CreateResponseViewModel(int statusCode) =>
                new ResponseViewModel(statusCode.ToString(),
                    CreatePropertyViewModels(samePathRequests, 
                    //                                                                                                    The first is the actual response
                        r => r.Responses.Where(res => res.StatusCode == statusCode).SelectMany(res => res.ResponseObjects.Take(1)).SelectMany(o => o.Properties)));
        }

        private ImmutableArray<ObjectViewModel> CreateResponseObjectViewModels(IEnumerable<NormalizedRequest> samePathRequests)
        {
            var objectViewModels = samePathRequests
                //                                                                 The first is the actual response, we skip it because it is in the Responses collection
                .SelectMany(r => r.Responses.SelectMany(res => res.ResponseObjects.Skip(1)))
                .Select(o => o.Name)
                .Distinct()
                .Select(CreateObjectViewModel)
                .ToImmutableArray();

            return objectViewModels;

            ObjectViewModel CreateObjectViewModel(string objectName) =>
                new ObjectViewModel(objectName,
                    CreatePropertyViewModels(samePathRequests,
                        //                                                     The first is the actual response, we skip it because it is in the Responses collection
                        r => r.Responses.SelectMany(res => res.ResponseObjects.Skip(1)).Where(o => o.Name == objectName).SelectMany(o => o.Properties)));
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
                .SelectMany(x => getProperties(x).Select(p => (x.Industry, x.Source, p)))
                .ToLookup(x => $"{x.Industry} {x.Source}", x => x.p);

            var viewModels = propertyNames
                .Select(name =>
                    new PropertyViewModel(
                        name,
                        CreatePropertyWithSourceViewModel("tobacco RU", name),
                        CreatePropertyWithSourceViewModel("ncp RU", name),
                        CreatePropertyWithSourceViewModel("otp RU", name),
                        CreatePropertyWithSourceViewModel("milk RU", name),
                        CreatePropertyWithSourceViewModel("water RU", name),
                        CreatePropertyWithSourceViewModel("beer RU", name),
                        CreatePropertyWithSourceViewModel("tobacco KZ", name),
                        CreatePropertyWithSourceViewModel("tobacco KG", name),
                        CreatePropertyWithSourceViewModel("tobacco UZ", name),
                        CreatePropertyWithSourceViewModel("beer UZ", name),
                        CreatePropertyWithSourceViewModel("water UZ", name)
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
