using Serilog;

namespace PieHandlerService.Api.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder AddSerilogConfiguration(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());
        Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
        return hostBuilder;
    }
}