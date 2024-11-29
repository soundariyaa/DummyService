namespace PieHandlerService.Api;

public static class Constants
{
    public static class Settings
    {
        public static string AppSettingsName => "appsettings";
        public static string AppSettingsJsonName => "appsettings.json";
    }

    public const string ServiceName = "PieHandlerService";


    public static class EnvironmentVariables
    {
        public static string AspNetCoreEnvironment => "ASPNETCORE_ENVIRONMENT";
        public const string AspNetVersion = "ASPNET_VERSION";
        public static string ElasticsearchUrl => "ELASTICSEARCH_URL";
        public static string ExecutionEnvironmentIdentifier => "EXECUTION_ENVIRONMENT_IDENTIFIER";
    }

    public static class CorsPolicy
    {
        public static string OpenCorsPolicy => "OpenCorsPolicy";
        public static string AnyGetCorsPolicy => "AnyGetCorsPolicy";
    }

    public static class ExpectedHeaderParameters
    {
        public const string XRequestId = "x-request-id";
    }

}