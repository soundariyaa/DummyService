using Newtonsoft.Json;
using System.Reflection;

namespace PieHandlerService.Application.Test;

public class EmbeddedResource
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
            var serializer = new JsonSerializer();
            return serializer.Deserialize<T>(jsonReader);
        }
    }

    public static class Raw
    {
        public static string GetEmbeddedResource(Assembly assembly, string embeddedResourcePathInAssembly)
        {
            var resourceCompletePath = assembly.GetName().Name + "." + embeddedResourcePathInAssembly;
            using var stream = assembly.GetManifestResourceStream(resourceCompletePath);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            return reader.ReadToEnd();
        }
    }
}