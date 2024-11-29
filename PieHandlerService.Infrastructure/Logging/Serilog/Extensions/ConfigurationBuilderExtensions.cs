using Microsoft.Extensions.Configuration;

namespace PieHandlerService.Infrastructure.Logging.Serilog.Extensions;

internal static class ConfigurationBuilderExtensions
{
    private const string DummyInMemoryFilePath = ".";
    public static IConfigurationBuilder AddInMemoryBase64EncodedJson(
        this IConfigurationBuilder configurationBuilder, string base64Json)
    {
        var memoryFileProvider = new InMemoryJsonFileProvider(Convert.FromBase64String(base64Json));
        configurationBuilder.AddJsonFile(memoryFileProvider, DummyInMemoryFilePath, false, false);
        return configurationBuilder;
    }
}
