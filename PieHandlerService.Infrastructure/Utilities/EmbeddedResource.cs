using Newtonsoft.Json;
using System.Reflection;

namespace PieHandlerService.Infrastructure.Utilities;

public static class EmbeddedResource
{
    public static class Json
    {
        public static T GetEmbeddedResource<T>(string embeddedResourcePathInAssembly) where T : class
        {
            var assembly = Assembly.GetExecutingAssembly();
            return GetEmbeddedResource<T>(assembly, embeddedResourcePathInAssembly);
        }

        public static T GetEmbeddedResource<T>(Assembly assembly, string embeddedResourcePathInAssembly) where T : class
        {
            var resourceCompletePath = assembly.GetName().Name + "." + embeddedResourcePathInAssembly;
            using var stream = assembly.GetManifestResourceStream(resourceCompletePath);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            using var jsonReader = new JsonTextReader(reader);
            var _serializer = new JsonSerializer();
            return _serializer.Deserialize<T>(jsonReader) ?? throw new ArgumentNullException(nameof(GetEmbeddedResource));
        }
    }
}