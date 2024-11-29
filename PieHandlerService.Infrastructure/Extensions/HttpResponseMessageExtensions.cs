using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace PieHandlerService.Infrastructure.Extensions;

internal static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccessStatusCode(
        this HttpResponseMessage response,
        ILogger logger)
    {
        if (response == null)
        {
            throw new ArgumentNullException($"Invalid {nameof(HttpResponseMessage)}. {nameof(response)} may not be null.");
        }

        if (response.Content != null)
        {
            logger.LogDebug("{MethodName} serialized raw response content: {Content}",
                nameof(EnsureSuccessStatusCode), JsonConvert.SerializeObject(await response.Content.ReadAsStringAsync()));
        }

        response.EnsureSuccessStatusCode();
    }

    public static bool IsEmpty(this HttpResponseMessage response) => response == default || response.RequestMessage == null || response.RequestMessage.IsEmpty();
}