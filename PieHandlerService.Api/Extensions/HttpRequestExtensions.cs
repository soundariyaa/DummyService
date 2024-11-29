namespace PieHandlerService.Api.Extensions;

public static class HttpRequestExtensions
{
    public static string PathAsString(this HttpRequest httpRequest)
    {
        try
        {
            if (!httpRequest.Host.HasValue || !httpRequest.Path.HasValue)
            {
                return string.Empty;
            }

            var path = string.Format($"{httpRequest.Scheme}://{httpRequest.Host.Value}{httpRequest.Path}");
            if (Uri.TryCreate(path, UriKind.Absolute, out var absolutePath))
            {
                return absolutePath.ToString();
            }

            return httpRequest.Path != null && httpRequest.Path.HasValue ? httpRequest.Path.Value : string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
