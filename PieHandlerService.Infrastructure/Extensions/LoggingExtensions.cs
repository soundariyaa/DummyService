using Microsoft.Extensions.Logging;

namespace PieHandlerService.Infrastructure.Extensions;

internal static class LoggingExtensions
{
    public static void LogException(this ILogger logger, Exception ex) => logger.LogError(ex, "{ErrorMessage}", ex.Message);

    public static void LogExceptionAsWarning(this ILogger logger, Exception ex) => logger.LogWarning(ex, "{ErrorMessage}", ex.Message);
}