using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace PieHandlerService.Infrastructure.Logging.Serilog.Extensions;

internal static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration WriteToElasticSearch(this LoggerConfiguration loggerConfiguration,
        string elasticSearchUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(elasticSearchUrl))
            {
                return loggerConfiguration;
            }

            var urlList = Array.Empty<string>();
            if (elasticSearchUrl.Contains(";") || elasticSearchUrl.Contains(","))
            {
                urlList = elasticSearchUrl.Split(';', ',');
            }

            loggerConfiguration = loggerConfiguration.WriteTo.Elasticsearch(
                urlList.Length > 0
                    ? new ElasticsearchSinkOptions(urlList.Select(x => new Uri(x)))
                    : new ElasticsearchSinkOptions(new Uri(elasticSearchUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                    });

            return loggerConfiguration;
        }
        catch (Exception)
        {
            return loggerConfiguration;
        }
    }
}
