using Microsoft.AspNetCore.HttpOverrides;

namespace PieHandlerService.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDefinedEnvironmentDependentExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        logger.LogInformation("{MethodName} - current environment execution: {EnvironmentName}",
            nameof(UseDefinedEnvironmentDependentExceptionHandling), env.EnvironmentName);

        if (env.IsDevelopment())
        {
            logger.LogInformation("{MethodName} - Initialize developerExceptionPage.",
                nameof(UseDefinedEnvironmentDependentExceptionHandling));
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
        }
        else
        {
            logger.LogInformation("{MethodName} - UseExceptionHandler for environment: {EnvironmentName}",
                nameof(UseDefinedEnvironmentDependentExceptionHandling), env.EnvironmentName);
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Internal Server Error");
                });
            });
        }
        return app;
    }

    public static IApplicationBuilder UseDefinedEnvironmentDependentHttps(this IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
    {
        logger.LogInformation("{MethodName} - current environment execution: {EnvironmentName}",
            nameof(UseDefinedEnvironmentDependentHttps), env.EnvironmentName);

        if (!env.IsProduction()) { return app; }
        logger.LogInformation("{MethodName} - Initializing HSTS. HSTS & HttpsRedirection only when in production environment.", nameof(UseDefinedEnvironmentDependentHttps));
        app.UseHsts();
        app.UseHttpsRedirection();
        return app;
    }
    public static IApplicationBuilder UseEmptyPathToSwagger(this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            if (string.IsNullOrEmpty(context.Request.Path.Value) || context.Request.Path.Value == "/")
            {
                context.Response.Redirect("/swagger/index.html", permanent: false);
            }
            else
            {
                await next();
            }
        });

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        return app
            .UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All })
            .UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Core.Constants.Settings.NamePieHandlerService} API");
            });

    }

}
