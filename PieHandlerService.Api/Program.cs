
using PieHandlerService.Api.Extensions;
using PieHandlerService.Infrastructure.Logging.Serilog;

namespace PieHandlerService.Api;

public class Program
{
    private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .AddAllConfigurations()
        .Build();

    public static void Main(string[] args)
    {

        var hostBuilder = CreateHostBuilder(args);

        new SerilogLoggingInitializerFactory()
            .Create(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.ElasticsearchUrl) ?? string.Empty)
            .Initialize(hostBuilder, Configuration);

        hostBuilder.Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            }); 

}