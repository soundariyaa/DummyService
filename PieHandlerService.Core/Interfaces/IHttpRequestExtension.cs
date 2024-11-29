namespace PieHandlerService.Core.Interfaces;

public interface IHttpRequestExtension
{

    HttpRequestMessage GetPreFlashSoftwareOrder(
        HttpRequestMessage request,
        HttpContent content,
        HttpClient httpClient,
        string url);

    HttpRequestMessage GetEndOfLineSoftwareOrder(
        HttpRequestMessage request,
        HttpContent content,
        HttpClient httpClient,
        string url);

    HttpRequestMessage GetFactoryVehicleCodes(
        HttpRequestMessage request,
        HttpContent content,
        HttpClient httpClient,
        string url);


}