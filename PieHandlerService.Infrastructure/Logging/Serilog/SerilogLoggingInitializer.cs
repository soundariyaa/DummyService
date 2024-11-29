using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Infrastructure.Logging.Serilog.Extensions;

namespace PieHandlerService.Infrastructure.Logging.Serilog;

internal sealed class SerilogLoggingInitializer(string elasticSearchUrl) : ILoggingInitializer
{
    public SerilogLoggingInitializer() : this(string.Empty) { }

    public void Initialize(IHostBuilder hostBuilder, IConfiguration configuration)
    {
        ConfigureSelfLog();
        CreateLogger(configuration);
        ConfigureHostBuilder(hostBuilder, configuration);
    }

    public bool TryInitialize(IHostBuilder hostBuilder, IConfiguration configuration)
    {
        try
        {
            Initialize(hostBuilder, configuration);
            return true;
        }
        catch { return false; }
    }

    private void CreateLogger(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration.OverrideSerilogDefaultConfiguration(out var overrideConfiguration)
                ? overrideConfiguration
                : configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("Application", nameof(PieHandlerService))
            .WriteToElasticSearch(elasticSearchUrl)
            .CreateLogger();
    }

    private static void ConfigureSelfLog() => global::Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

    private static void ConfigureHostBuilder(IHostBuilder hostBuilder, IConfiguration config)
    {
        hostBuilder
            .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
            .ConfigureAppConfiguration(builder => builder.AddConfiguration(config))
            .UseSerilog();
    }
}
