using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using PieHandlerService.Infrastructure.Extensions;
using PieHandlerService.Application.Extensions;
using PieHandlerService.Core;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Api.Converters;

namespace PieHandlerService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRConfigurations(this IServiceCollection services) =>
        services.AddMediatR(configuration =>
                configuration.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));


    public static IServiceCollection AddMemoryCacheConfiguration(this IServiceCollection services)
    {
        services.AddMemoryCache(setupAction => setupAction.ExpirationScanFrequency = TimeSpan.FromSeconds(30));
        return services;
    }

    public static IServiceCollection AddApiVersioningConfigurations(this IServiceCollection services)
    {
        // "api-supported-versions: 1.0" in response header
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });
        return services;
    }

    public static IServiceCollection AddControllerWithJsonSettings(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.Formatting = Formatting.Indented;
        });


        return AddGlobalJsonCamelCaseConfiguration(services);
    }

    public static IServiceCollection AddAutoMapperForMultipleAssemblies(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMapperProfile));
        return services;
    }

    private static IServiceCollection AddGlobalJsonCamelCaseConfiguration(this IServiceCollection services)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        return services;
    }


    public static IServiceCollection AddCorsConfigurations(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Constants.CorsPolicy.OpenCorsPolicy,
                builder => builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            options.AddPolicy(Constants.CorsPolicy.AnyGetCorsPolicy,
                builder => builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .WithMethods("GET"));
        });
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpContextAccessor();
        services.AddScoped(typeof(CancellationToken), serviceProvider =>
        {
            IHttpContextAccessor httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return httpContext.HttpContext?.RequestAborted ?? CancellationToken.None;
        });
        services.AddScoped<IProblemDetailsManager, ProblemDetailsManager>();
        return services;
    }


    public static IServiceCollection AddSwaggerConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var version = GetDeploymentVersion(configuration);
        var serviceDeploymentMetadata = "";
        if (!string.IsNullOrWhiteSpace(version))
        {
            serviceDeploymentMetadata += $", Deploy: {version}";
        }

        var executionEnvironmentIdentifier = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.ExecutionEnvironmentIdentifier) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(executionEnvironmentIdentifier))
        {
            serviceDeploymentMetadata += $", Execution Environment: {executionEnvironmentIdentifier}";
        }

        services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"{Core.Constants.Settings.TitleFriendlyNamePieHandlerService} API",
                Version = $"Api: v1{serviceDeploymentMetadata}",
                Description =
                    $"{Core.Constants.Settings.NamePieHandlerService} generates End of Line, PreFlash order information and vehicle codes/vehicle object with processed broadcast file as input."
            });
        });
        return services;
    }


    public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        return services.ConfigureInfrastructure(configuration);
    }

    public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services)
    {
        return services.ConfigureApplication();
    }


    private static string GetDeploymentVersion(IConfiguration configuration)
    {
        try
        {
            return Environment.GetEnvironmentVariable(Core.Constants.Settings.SemanticVersion) ?? string.Empty;
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
