using Microsoft.Extensions.DependencyInjection;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Application.Services;
using PieHandlerService.Core;


namespace PieHandlerService.Application.Extensions;

public static class ServiceCollectionExtension
{

    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        return services.AddApplicationServices();

    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderCreationService, OrderCreationService>();
        services.AddScoped<IOrderDetailService, OrderDetailService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IMessageDetailService, MessageDetailService>();
        services.AddScoped<IProblemDetailsManager, ProblemDetailsManager>();
          
        return services;
    }
}