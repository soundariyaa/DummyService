namespace PieHandlerService.Core.Interfaces;

public interface IHttpResponseExtension
{
    Task<T> ContentAsType<T>(HttpResponseMessage response);

    Task<string> ContentAsJson(HttpResponseMessage response);

    Task<string> ContentAsString(HttpResponseMessage response);
}
