using Newtonsoft.Json;

namespace PieHandlerService.Core.Extensions;

public static class GenericExtensions
{
    public static T Clone<T>(this T source)
    {
        var serializedSource = JsonConvert.SerializeObject(source);
        var clonedObject = JsonConvert.DeserializeObject<T>(serializedSource);
        return clonedObject != null ? clonedObject : source;
    }
}
