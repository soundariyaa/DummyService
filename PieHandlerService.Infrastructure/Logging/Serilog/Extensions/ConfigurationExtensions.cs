using Microsoft.Extensions.Configuration;
using PieHandlerService.Core.Extensions;

namespace PieHandlerService.Infrastructure.Logging.Serilog.Extensions;

internal static class ConfigurationExtensions
{
    public static bool OverrideSerilogDefaultConfiguration(this IConfiguration configuration, out IConfiguration? overrideConfigurationResult)
    {
        overrideConfigurationResult = null;
        try
        {
            var overrideConfigurationBase64Json = configuration
                .GetSection(Constants.EnvironmentVariables.SerilogOverrideDefaultConfigurationBase64Json)?.Value;

            if (overrideConfigurationBase64Json == null ||
                !overrideConfigurationBase64Json.IsValidBase64EncodedString())
            {
                return false;
            }
            overrideConfigurationResult = new ConfigurationBuilder().AddInMemoryBase64EncodedJson(overrideConfigurationBase64Json).Build();
            return overrideConfigurationResult != null;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
