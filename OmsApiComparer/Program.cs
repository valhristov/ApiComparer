using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using OmsApiComparer.Swagger;

namespace OmsApiComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var requests = SwaggerAdapter.Read(File.ReadAllText("ru.json"));

            foreach (var request in requests)
            {
                CreateClient(request);
            }
        }

        private static string CreateClient(ApiRequest request)
        {
            return "";
        }
    }
}
