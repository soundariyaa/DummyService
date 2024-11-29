using Newtonsoft.Json;
using PieHandlerService.Core.Interfaces;

namespace PieHandlerService.Core.Extensions;

internal class HttpResponseExtensionsImpl : IHttpResponseExtension
{
    public async Task<T> ContentAsType<T>(HttpResponseMessage response)
    {
        var data = await response.Content.ReadAsStringAsync();
       
        return JsonConvert.DeserializeObject<T>(data) ?? throw new ArgumentNullException(nameof(data));

    }

    public async Task<string> ContentAsJson(HttpResponseMessage response)
    {
        var data = await response.Content.ReadAsStringAsync();

        return JsonConvert.SerializeObject(data);
    }

    public async Task<string> ContentAsString(HttpResponseMessage response)
    {
        return await response.Content.ReadAsStringAsync();
    }
}

public static class HttpResponseExtensions
{
    internal static IHttpResponseExtension Implementation { private get; set; } = new HttpResponseExtensionsImpl();

    public static Task<T> ContentAsType<T>(this HttpResponseMessage response)
    {
        return Implementation.ContentAsType<T>(response);
    }

    public static Task<string> ContentAsJson(this HttpResponseMessage response)
    {
        return Implementation.ContentAsJson(response);
    }

    public static Task<string> ContentAsString(this HttpResponseMessage response)
    {
        return Implementation.ContentAsString(response);
    }
}
