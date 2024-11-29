using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace PieHandlerService.Infrastructure.Logging.Serilog.Extensions;

public static class LoggerExtensions
{
    private static string CorrelationIdKey => "CorrelationIdentifier";

    public static IDisposable PushProperty(this ILogger _, string correlationId)
    {
        return LogContext.PushProperty(CorrelationIdKey, correlationId, true);
    }
}
