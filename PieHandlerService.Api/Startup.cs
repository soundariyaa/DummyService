using AutoMapper;
using Prometheus;
using PieHandlerService.Api.Extensions;

namespace PieHandlerService.Api;

public sealed class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        services.AddOptions();
        services.AddHttpClient();
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
        services.AddHttpContextAccessor();
        services.AddMediatRConfigurations();
        services.AddApiVersioningConfigurations();
        services.AddControllerWithJsonSettings();
        services.AddCorsConfigurations();
        services.AddSwaggerConfigurations(Configuration);
        services.AddInfrastructureConfiguration(Configuration);
        services.AddApplicationConfiguration();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IMapper mapper)
    {
        logger?.LogInformation("Configure {ClassName} for application: {ApplicationName} in environment: {EnvironmentName}",
            nameof(Startup), env.ApplicationName, env.EnvironmentName);

        _ = app
            .UseRouting()
            .UseResponseCompression();

        app.UseMetricServer(cfg => cfg.EnableOpenMetrics = false);
        app.UseResponseCompression();
        app.UseHttpMetrics();
        app.UseEmptyPathToSwagger();
        app.UseSwaggerConfiguration();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}