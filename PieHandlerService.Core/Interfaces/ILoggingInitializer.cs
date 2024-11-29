using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace PieHandlerService.Core.Interfaces;

public interface ILoggingInitializer
{
    bool TryInitialize(IHostBuilder hostBuilder, IConfiguration config);
    void Initialize(IHostBuilder hostBuilder, IConfiguration config);
}
