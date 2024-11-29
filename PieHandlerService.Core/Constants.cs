namespace PieHandlerService.Core;

public sealed class Constants
{
    public static class Settings
    {
        public static string NamePieHandlerService => "PieHandlerService";
        public static string TitleFriendlyNamePieHandlerService => "PieHandler Service";
        public static string KeyDefaultLoggingFolder => "DEFAULT_LOGGING_FOLDER";
        public static string KeyAspNetCoreVersion => "DOTNET_VERSION";
        public static string KeyAspNetCoreEnvironment => "ASPNETCORE_ENVIRONMENT";
        public static string KeyDotNetRunningInContainer => "DOTNET_RUNNING_IN_CONTAINER";
        public static string KeyContainerImageName => "CONTAINER_IMAGE_NAME";
        public static string KeyPodName => "POD_NAME";
        public static string SemanticVersion => "SEMANTIC_VERSION";
        public static string KeyBranchName => "BRANCH_NAME";
    }

    public static class ExpectedCustomHttpHeaderParameters
    {
        public static string AcceptLanguage => "Accept-Language";
        public static string Authorization => "Authorization";
        public static string Cookie => "Cookie";
        public static string XRoute => "x-route";
        public static string XVersion => "x-version";
        public static string XRequestId => "x-request-id";
        public static string Traceparent => "traceparent";
    }

    public static class Default
    {
        public static class Timing
        {
            public static int GlobalDataCacheMaxValidPeriodInMinutes => 180;
        }
    }

    public static class HttpClientConfigurations
    {
        public static string GetPreFlashOrderHttpClient => "GetPreFlashOrderHttpClient";
        public static string GetEndOfLineOrderHttpClient => "GetEndOfLineOrderHttpClient";
        public static string GetVehicleCodesHttpClient => "GetVehicleCodesHttpClient";

    }
}