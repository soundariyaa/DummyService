using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PieHandlerService.Core.Interfaces;


namespace PieHandlerService.Infrastructure.Extensions;

internal class HttpRequestExtensionImpl : IHttpRequestExtension
{
    private static void CopyHeaders( HttpRequestMessage request, bool includeAuthorization)
    {
        if (includeAuthorization)
        {
            request.Headers.Add(Core.Constants.ExpectedCustomHttpHeaderParameters.Authorization, String.Format("Bearer {0}",Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.PieVTSToken)));
            request.Headers.Add(Core.Constants.ExpectedCustomHttpHeaderParameters.XRoute, Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.XRoute));
        }
    }

    public HttpRequestMessage GetPreFlashSoftwareOrder(
        HttpRequestMessage request,
        HttpContent content,
        HttpClient httpClient,
        string url)
    {
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(httpClient.BaseAddress + url);
        request.Content = content;
        CopyHeaders( request, includeAuthorization: true);
        return request;
    }

    public HttpRequestMessage GetFactoryVehicleCodes(
    HttpRequestMessage request,
    HttpContent content,
    HttpClient httpClient,
    string url)
    {
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(httpClient.BaseAddress + url);
        request.Content = content;
        CopyHeaders( request, includeAuthorization: true);
        return request;
    }

    public HttpRequestMessage GetEndOfLineSoftwareOrder(HttpRequestMessage request, HttpContent content, HttpClient httpClient, string url)
    {
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(httpClient.BaseAddress + url);
        request.Content = content;
        CopyHeaders( request, includeAuthorization: true);
        return request;
    }
}


public static class HttpRequestExtension
{
    internal static IHttpRequestExtension Implementation { private get; set; } = new HttpRequestExtensionImpl();

    public static HttpRequestMessage GetPreFlashSoftwareOrder(
        this HttpRequestMessage request,
        HttpContent content,
        HttpClient httpClient,
        string url)
    {
        return Implementation == null
            ? new HttpRequestMessage()
            : Implementation.GetPreFlashSoftwareOrder(request, content, httpClient, url);
    }

    public static HttpRequestMessage GetEndOfLineSoftwareOrder(
    this HttpRequestMessage request,
    HttpContent content,
    HttpClient httpClient,
    string url)
    {
        return Implementation == null
            ? new HttpRequestMessage()
            : Implementation.GetEndOfLineSoftwareOrder(request, content, httpClient, url);
    }

    public static HttpRequestMessage GetFactoryVehicleCodes(
    this HttpRequestMessage request,
    HttpContent content,
    HttpClient httpClient,
    string url)
    {
        return Implementation == null
            ? new HttpRequestMessage()
            : Implementation.GetFactoryVehicleCodes(request, content, httpClient, url);
    }

    public static void LogRequest(this HttpRequestMessage request, string methodName, ILogger logger)
    {
        logger.LogInformation("{MethodName} Serialized request: {SerializedRequest}",
            methodName, JsonConvert.SerializeObject(request.Content));
    }


    public static bool IsEmpty(this HttpRequestMessage request) => request == default || request.Content == null && request.RequestUri == default;

}


