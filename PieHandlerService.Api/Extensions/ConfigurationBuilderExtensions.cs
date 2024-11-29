namespace PieHandlerService.Api.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddAllConfigurations(this ConfigurationBuilder configBuilder)
    {
        return configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Constants.Settings.AppSettingsJsonName, false, true)
            .AddJsonFile(
                $"{Constants.Settings.AppSettingsName}.{Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.AspNetCoreEnvironment)}.json", true, true)
            .AddEnvironmentVariables();
    }
}
