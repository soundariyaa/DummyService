using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PieHandlerService.Core.Extensions;
using PieHandlerService.Infrastructure.Factories;
using PieHandlerService.Infrastructure.Services.Pie;
using Polly;
using static PieHandlerService.Core.Constants;
using System.Net.Http.Headers;
using PieHandlerService.Infrastructure.Repositories.Order;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Text;
using PieHandlerService.Infrastructure.Services.Storage.Service;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using PieHandlerService.Infrastructure.Services.Messaging.Service;
using PieHandlerService.Infrastructure.Services.Messaging.Interfaces;
using Microsoft.Extensions.Hosting;
using PieHandlerService.Infrastructure.Services.Helper;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Interfaces;
using PieHandlerService.Infrastructure.Services;
using Microsoft.Extensions.Options;
using PieHandlerService.Infrastructure.Services.Messaging;
using System.Text.Json;
using PieHandlerService.Core.Interface;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using PieHandlerService.Infrastructure.Repositories.Message;
using PieHandlerService.Infrastructure.Repositories;

namespace PieHandlerService.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    private static readonly JsonSerializerOptions SerializationOptions =
        new() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddPieService()
            .AddPersistantLayer(configuration)
            .AddStorageService()
            .AddMQService()
            .AddMetricsService()
            .AddIbmMqConfiguration(configuration)
            .AddHostConfiguration()
            .AddAutoMapper(typeof(RepoMapperProfile))
            .AddHttpClientConfigurations(configuration)
            .AddDatabaseConfigurations(configuration)
            .AddMQConfiguration();
    }


    private static IServiceCollection AddPieService(this IServiceCollection services)
    {
        services.AddScoped<IProblemDetailHandler, ProblemDetailsHandler>();
        services.AddScoped<ITimingConfigurationInformer, TimingConfigurationInformer>();
        services.AddScoped<ISoftwareProviderFactory, PieSoftwareProviderFactory>();
        services.AddScoped<ICertificateChainService, CertificateChainService>();
        services.AddScoped<ISoftwareMetadataService, PieSoftwareMetadataService>();
        return services;
    }

    private static IServiceCollection AddMetricsService(this IServiceCollection services)
    {
        services.AddScoped<IMetricsService, MetricsService>();
        return services;
    }


    private static IServiceCollection AddStorageService(this IServiceCollection services)
    {
        services.AddScoped<IVbfStorageHandler, VbfStorageHandler>();
        services.AddScoped<IBroadcastContextStorageHandler, BroadcastContextStorageHandler>();
        services.AddScoped<ISiigOrderStorageHandler, SiigOrderStorageHandler>();
        services.AddScoped<IStorageHandlerService, StorageHandlerService>();
        services.AddScoped<IStorageOperation, NasStorageOperation>();
        return services;
    }


    private static IServiceCollection AddMQService(this IServiceCollection services)
    {
        services.AddScoped<IMqConnectionFactory, MqConnectionFactory>();
        return services;
    }

    private static IServiceCollection AddHostConfiguration(this IServiceCollection services) => services
        .Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        })
        .AddSingleton<IIBChannelHandlerService, IBChannelHandlerService>()
        .AddHostedService<IBChannelHandlerService>(serviceProvider =>
            (serviceProvider.GetRequiredService<IIBChannelHandlerService>() as IBChannelHandlerService)!)
        .AddSingleton<IOBChannelHandlerService, OBChannelHandlerService>()
        .AddHostedService<OBChannelHandlerService>(serviceProvider =>
            (serviceProvider.GetRequiredService<IOBChannelHandlerService>() as OBChannelHandlerService)!);


    private static IServiceCollection AddHttpClientConfigurations(this IServiceCollection services,
        IConfiguration configuration)
    {
        var httpClientTimeOut = GetCommonHttpClientTimeout(configuration);
        var circuitBreakerTimeout = GetCommonCircuitBreakerTimeout(configuration);
        var circuitBreakerRetriesBeforeOpening = GetCommonCircuitRetriesBeforeOpening(configuration);

        _ = services.AddHttpClient(HttpClientConfigurations.GetVehicleCodesHttpClient, client =>
            {
                var pieServiceUri = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.PieServiceUrl) ?? throw new ArgumentNullException(nameof(Constants.EnvironmentVariables.PieServiceUrl)); ;
                client.BaseAddress = new Uri(pieServiceUri.EnsureTrailingSlash());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(
                    Constants.RequestHeaders.ApiGatewayUserKey, configuration[Constants.EnvironmentVariables.PieServiceApiGatewayUserKey]);
                client.Timeout = TimeSpan.FromSeconds(httpClientTimeOut); // dns lookup can be time consuming
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2)
                }))
            .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(circuitBreakerRetriesBeforeOpening, TimeSpan.FromSeconds(circuitBreakerTimeout)));


        _ = services.AddHttpClient(HttpClientConfigurations.GetPreFlashOrderHttpClient, client =>
            {
                var pieServiceUri = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.PieServiceUrl) ?? throw new ArgumentNullException(nameof(Constants.EnvironmentVariables.PieServiceUrl));
                client.BaseAddress = new Uri(pieServiceUri.EnsureTrailingSlash());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(
                    Constants.RequestHeaders.ApiGatewayUserKey, configuration[Constants.EnvironmentVariables.PieServiceApiGatewayUserKey]);
                client.Timeout = TimeSpan.FromSeconds(httpClientTimeOut); // dns lookup can be time consuming
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2)
                }))
            .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(circuitBreakerRetriesBeforeOpening, TimeSpan.FromSeconds(circuitBreakerTimeout)));

        _ = services.AddHttpClient(HttpClientConfigurations.GetEndOfLineOrderHttpClient, client =>
            {
                var pieServiceUri = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.PieServiceUrl) ?? throw new ArgumentNullException(nameof(Constants.EnvironmentVariables.PieServiceUrl));
                client.BaseAddress = new Uri(pieServiceUri.EnsureTrailingSlash());
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(
                    Constants.RequestHeaders.ApiGatewayUserKey, configuration[Constants.EnvironmentVariables.PieServiceApiGatewayUserKey]);
                client.Timeout = TimeSpan.FromSeconds(httpClientTimeOut); // dns lookup can be time consuming
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2)
                }))
            .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(circuitBreakerRetriesBeforeOpening, TimeSpan.FromSeconds(circuitBreakerTimeout)));
        return services;
    }

    private static IServiceCollection AddIbmMqConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var disableMq =
            (configuration.GetSection(Constants.EnvironmentVariables.MqDisableMq).Value ?? "false")
            .Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);

        var mqConfigurationBase64Json = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.MqConfigurationBase64Json);

        var mqConfigurationJson = !String.IsNullOrEmpty(mqConfigurationBase64Json) ? 
            Encoding.UTF8.GetString(Convert.FromBase64String(mqConfigurationBase64Json)) : throw new ArgumentNullException(nameof(mqConfigurationBase64Json));

        var mqConfiguration = System.Text.Json.JsonSerializer.Deserialize<MqConfiguration>(mqConfigurationJson, SerializationOptions)
                              ?? throw new ArgumentNullException(nameof(mqConfigurationJson));
        mqConfiguration.DisableMq = disableMq;
        if (!mqConfiguration.IsValid())
        {
            throw new InvalidOperationException($"Invalid IBM MQ configuration: {mqConfigurationJson}");
        }
        var ibmMqConfigurationOptions = new OptionsWrapper<MqConfiguration>(mqConfiguration);
        services.AddSingleton<IOptions<MqConfiguration>>(ibmMqConfigurationOptions);
        return services;
    }

    private static readonly string MqClientId = $"{nameof(PieHandlerService)}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    private static string GetMqClientId(MqConfiguration mqConfiguration) =>
        !string.IsNullOrWhiteSpace(mqConfiguration.ClientId)
            ? $"{mqConfiguration.ClientId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}" : MqClientId;

    private static double GetCommonHttpClientTimeout(IConfiguration configuration)
    {
        var httpClientTimeOut = Constants.DefaultTimeouts.DefaultHttpClientTimeoutInSeconds;
        var requestedHttpTimeout = configuration[Constants.EnvironmentVariables.CommonHttpClientTimeoutInSeconds] ?? "";
        if (!string.IsNullOrWhiteSpace(requestedHttpTimeout) &&
            double.TryParse(requestedHttpTimeout, out var result))
        {
            httpClientTimeOut = result;
        }
        return httpClientTimeOut;
    }

    private static double GetCommonCircuitBreakerTimeout(IConfiguration configuration)
    {
        var circuitBreakerTimeout = Constants.DefaultTimeouts.DefaultCircuitBreakerTimeoutInSeconds;
        var requestedCircuitBreakerTimeout = configuration[Constants.EnvironmentVariables.CommonCircuitBreakerTimeoutInSeconds] ?? "";
        if (!string.IsNullOrWhiteSpace(requestedCircuitBreakerTimeout) &&
            double.TryParse(requestedCircuitBreakerTimeout, out var result))
        {
            circuitBreakerTimeout = result;
        }
        return circuitBreakerTimeout;
    }

    private static IServiceCollection AddMQConfiguration(this IServiceCollection services)
    {
        var base64String = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.MqConnectionProperties) ?? Constants.DefaultMqConstants.DefaultMqConfiguration;
        var mqConnectionPropertiesJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

        if (string.IsNullOrWhiteSpace(mqConnectionPropertiesJson))
        {
            throw new ArgumentException($"{nameof(Constants.EnvironmentVariables.MqConnectionProperties)} cannot be null or empty");
        }

        var base64AutoReconnect = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.MqAutoReconnectProperties) ?? Constants.DefaultMqConstants.DefaultMqAutoReconnect;
        var mqAutoReconnectPropertiesJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64AutoReconnect));
        if (string.IsNullOrWhiteSpace(mqAutoReconnectPropertiesJson))
        {
            throw new ArgumentException($"{nameof(Constants.EnvironmentVariables.MqAutoReconnectProperties)} cannot be null or empty");
        }


        var base64CustomReconnect = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.MqCustomReconnectProperties) ?? Constants.DefaultMqConstants.DefaultMqCustomReconnect;
        var mqCustomReConnectPropertiesJson = Encoding.UTF8.GetString(Convert.FromBase64String(base64CustomReconnect));
        if (string.IsNullOrWhiteSpace(mqCustomReConnectPropertiesJson))
        {
            throw new ArgumentException($"{nameof(Constants.EnvironmentVariables.MqCustomReconnectProperties)} cannot be null or empty");
        }

        return services;
    }


    private static int GetCommonCircuitRetriesBeforeOpening(IConfiguration configuration)
    {
        var circuitBreakerRetriesBeforeOpening = Constants.DefaultRetries.DefaultCircuitNumberOfRetriesBeforeOpening;
        var requestedCircuitBreakerRetriesBeforeOpening = configuration[Constants.EnvironmentVariables.CommonCircuitBreakerRetriesBeforeOpening] ?? "";
        if (!string.IsNullOrWhiteSpace(requestedCircuitBreakerRetriesBeforeOpening) &&
            int.TryParse(requestedCircuitBreakerRetriesBeforeOpening, out var result))
        {
            circuitBreakerRetriesBeforeOpening = result;
        }
        return circuitBreakerRetriesBeforeOpening;
    }

    internal static IServiceCollection AddPersistantLayer(this IServiceCollection services, IConfiguration configuration)
    {
        const string conventionName = "CamelCase";

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        ConventionRegistry.Register(conventionName, new ConventionPack { new CamelCaseElementNameConvention() }, _ => true);
        services.Configure<DatabaseSettings>(configure =>
        {
            configure.ConnectionString = configuration.GetSection(Constants.EnvironmentVariables.MongoDbConnectionString).Value;
            ArgumentNullException.ThrowIfNull(configure.ConnectionString, nameof(configure.ConnectionString));
            configure.Database = configuration.GetSection(Constants.EnvironmentVariables.MongoDatabaseName).Value;
            ArgumentNullException.ThrowIfNull(configure.Database, nameof(configure.Database));
        });

        services.AddSingleton<IMongoClient, MongoClient>(serviceProvider =>
        {
            var mongoDbSetting = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>();
            return new MongoClient(MongoClientSettings.FromUrl(new MongoUrl(mongoDbSetting.Value.ConnectionString)));
        });
        return services
            .AddScoped<ISiigOrderRepository, SiigOrderRepository>()
            .AddScoped<IPieOrderRepository, PieOrderRepository>()
            .AddScoped<IBroadcastMetadataRepository, BroadcastMetadataRepository>()
            .AddScoped<IRepositoryModifier<Core.Models.PieResponseMessage>, PieOrderRepository>()
            .AddScoped<IRepositoryReader<PieMessageSpecification, Core.Models.PieResponseMessage>, PieOrderRepository>()
            .AddScoped<IRepositoryModifier<Core.Models.BroadcastContextMessage>, BroadcastMetadataRepository>()
            .AddScoped<IRepositoryReader<BroadcastMessageSpecification, Core.Models.BroadcastContextMessage>, BroadcastMetadataRepository>()
            .AddScoped<IRepositoryModifier<Core.Models.SiigOrder>, SiigOrderRepository>()
            .AddScoped<IRepositoryReader<SiigOrderQuerySpecification, Core.Models.SiigOrder>, SiigOrderRepository>();
    }

    internal static IServiceCollection AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configure =>
        {
            configure.ConnectionString
                = configuration.GetSection(Constants.EnvironmentVariables.MongoDbConnectionString).Value;
            configure.Database
                = configuration.GetSection(Constants.EnvironmentVariables.MongoDatabaseName).Value;
        });
        var dbConfig = new DatabaseSettings();
        dbConfig.ConnectionString = configuration.GetSection(Constants.EnvironmentVariables.MongoDbConnectionString).Value;
        dbConfig.Database = configuration.GetSection(Constants.EnvironmentVariables.MongoDatabaseName).Value;
        var ibmMqConfigurationOptions = new OptionsWrapper<DatabaseSettings>(dbConfig);
        services.AddSingleton<IOptions<DatabaseSettings>>(ibmMqConfigurationOptions);
        return services;
    }
}