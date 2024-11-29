using PieHandlerService.Core.Interfaces;

namespace PieHandlerService.Infrastructure.Logging.Serilog;

public sealed class SerilogLoggingInitializerFactory : ILoggingInitializerFactory
{
    public ILoggingInitializer Create(string elasticSearchUrl)
    {
        return new SerilogLoggingInitializer(elasticSearchUrl);
    }

    public ILoggingInitializer Create()
    {
        return new SerilogLoggingInitializer();
    }
}
