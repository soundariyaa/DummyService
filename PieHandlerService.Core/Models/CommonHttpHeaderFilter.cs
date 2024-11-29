namespace PieHandlerService.Core.Models;

public sealed class CommonHttpHeaderFilter
{
    public AcceptLanguage? AcceptLanguage { get; set; }
    public Authorization? Authorization { get; set; }
    public string? XRoute { get; set; }
    public string? XVersion { get; set; }
    public string? XRequestId { get; set; }
    public string? Traceparent { get; set; }
}