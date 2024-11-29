using MongoDB.Driver;

namespace PieHandlerService.Infrastructure;

public class Constants
{
    public static class EnvironmentVariables
    {
        public static string FactoryIdentifier = "FACTORY_IDENTIFIER";
        public static string DeploymentEnvironment = "DEPLOYMENT_ENVIRONMENT";

        public static string RetryCount = "RETRY_COUNT";
        public static string RetryDelaySeconds = "RETRY_DELAYSECONDS";


        public static string PieVbfValidation = "PIE_VBF_VALIDATION";

        public static string GlobalDataCacheMaxValidPeriodInMinutes => "GLOBAL_DATA_CACHE_MAX_VALID_PERIOD_IN_MINUTES";
        public static string InMemoryDataStore => "IN_MEMORY_DATA_STORE";
        public static string MongoDbConnectionString => "MONGODB_CONNECTION_STRING";
        public static string MongoDatabaseName => "MONGO_DATABASE_NAME";

        public static string CommonHttpClientTimeoutInSeconds => "COMMON_HTTP_CLIENT_TIMEOUT_IN_SECONDS";
        public static string CommonCircuitBreakerTimeoutInSeconds => "COMMON_CIRCUIT_BREAKER_TIMEOUT_IN_SECONDS";
        public static string CommonCircuitBreakerRetriesBeforeOpening => "COMMON_CIRCUIT_BREAKER_RETRIES_BEFORE_OPENING";
        public static string SerilogOverrideDefaultConfigurationBase64Json => "SERILOG_OVERRIDE_DEFAULT_CONFIGURATION_BASE64_JSON";

        public static string NASPIEStorageLocation => "NAS_PIE_STORAGELOCATION";
        public static string NASPIEVbfLocation => "NAS_PIE_VBFLOCATION";
        public static string NASPIEVinUniqueLocation => "NAS_PIE_VINUNIQUE_LOCATION";
        public static string NASOEVinUniqueLocationPreFix => "NAS_OE_VINUNIQUE_LOCATION_PREFIX";
        public static string NASOrderLocation => "NAS_ORDER_LOCATION";
        public static string NASBroadcastContextLocation => "NAS_BROADCAST_CONTEXT_LOCATION";

        public static string NASPreFlashSubDirectory => "NAS_PREFLASH_SUBDIRECTORY";
        public static string NASEndOfLineSubDirectory => "NAS_ENDOFLINE_SUBDIRECTORY";
        public static string NASVehicleCodesSubDirectory => "NAS_VEHICLECODES_SUBDIRECTORY";
        public static string NASVehicleObjectSubDirectory => "NAS_VEHICLEOBJECT_SUBDIRECTORY";

        public static string MqConnectionProperties => "MQ_CONNECTION_PROPERTIES";
        public static string MqAutoReconnectProperties => "MQ_AUTORECONNECT_PROPERTIES";
        public static string MqCustomReconnectProperties => "MQ_CUSTOMRECONNECT_PROPERTIES";
        public static string MqConfigurationBase64Json = "MQ_CONFIGURATION_BASE64_JSON";
        public static string MqDisableMq = "MQ_DISABLED";

        public static string ExampleMixNumber => "1231441";
        public static string OrderPublishQueue => "PIEORDERPUBLISHQUEUE";

        public static string PieTraceEnabled => "PIE_TRACE_ENABLED";
        public static string PieServiceUrl => "PIE_SERVICE_URL";
        public static string PieServiceApiGatewayUserKey => "PIE_SERVICE_APIGATEWAY_USER_KEY";
        public static string PieVTSToken => "PIE_VTS_TOKEN";
        public static string XRoute => "X_ROUTE";

        public const string VtsLongLivedToken = "VTS_LONG_LIVED_TOKEN";

        public const string FactoryCertChain = "FACTORY_CERT_CHAIN";
    }

    public static class Factory {
        public static string CertificatePrefix = "oefactory";
        public static string PieRequest = "Input";
        public static string PieResponse = "Output";
        public static string PieSuccess = "Success";
        public static string PieFailure = "Failure";
        public static string VinUnique = "vinunique";
        public static string EOLZone = "EOL";
        public static string VehicleCodesSessionKeyId => "keyForVehicleCodes";
        public static string VbfSessionKeyId => "keyForVbfs";
    }

    public static class Metrics
    {
        /**
         * PIE Integration metrics
         */
        public static string HandledPieVehicleObjectRequestsMetric => "piehandlerservice_handled_vehicleobjectrequests";
        public static string HandledPieEndOfLineRequestsMetric => "piehandlerservice_handled_endoflinerequests";
        public static string HandledPiePreFlashRequestsMetric => "piehandlerservice_handled_preflashrequests";
            
        /**
         * SIIG order storage handler metrics
         */
        public static string HandledStorageVehicleObjectOrderMetric => "piehandlerservice_storagesiigorder_vehicleobject";
        public static string HandledStorageEndOfLineOrderMetric => "piehandlerservice_storagesiigorder_endofline";
        public static string HandledStoragePreFlashOrderMetric => "piehandlerservice_storagesiigorder_preflash";
            
        /**
         * Broadcast Storage Handler metrics
         */
        public static string HandledStorageVehicleObjectBroadcastMetric => "piehandlerservice_storagebroadcasts_vehicleobjects";
        public static string HandledStorageEndOfLineBroadcastMetric => "piehandlerservice_storagebroadcasts_endofline";
        public static string HandledStoragePreFlashBroadcastMetric => "piehandlerservice_storagebroadcasts_preflash";
            
        /**
         * Database persistence metrics
         */
        public static string HandledBroadcastContextMessageDataMetric => "piehandlerservice_broadcastcontextmessage_data";
        public static string HandledPieResponseMessageDataMetric => "piehandlerservice_pieresponsemessage_data";
        public static string HandledSiigOrderDataMetric => "piehandlerservice_siigorder_data";

        /**
        * Message Queue metrics
        */
        public static string HandledInboundQueueMetric => "piehandlerservice_inboundqueue_data";
        public static string HandledOutboundQueueMetric => "piehandlerservice_outboundqueue_data";

        /**
        * PIE VBF software metrics
        */
        public static string HandledPieMissingVbfs => "piehandlerservice_missingvbf_files";
        public static string HandledPieErrorCode => "piehandlerservice_errorcode_data";

        public static string ResultKey => "result";
        public static string EndpointKey => "endpoint";
        public static string SuccessValue => "SUCCESS";
        public static string FailureValue => "FAILURE";
    }

    public static class DefaultTimeouts
    {
        public static double DefaultHttpClientTimeoutInSeconds => 20; // dns lookup can take time
        public static double DefaultCircuitBreakerTimeoutInSeconds => 10;
    }

    public static class DefaultRetries
    {
        public static int DefaultCircuitNumberOfRetriesBeforeOpening => 6;
    }

    public static class DefaultMqConstants {

        public static string DefaultMqConfiguration => "ew0KICAgICJob3N0IjogImlibW1xIiwNCiAgICAicG9ydCI6IDE0MTQsDQogICAgInF1ZXVlTWFuYWdlciI6ICJRTTEiLA0KICAgICJjaGFubmVsIjogIkRFVi5BUFAuU1ZSQ09OTiIsDQogICAgImNvbm5lY3Rpb25Nb2RlIjogMSwNCiAgICAiY2xpZW50QWNrbm93bGVkZ2UiOiB0cnVlLA0KICAgICJxdWV1ZU5hbWUiOiAiREVWLlRPLlExIiwNCiAgICAidHJ5UmVjb25uZWN0IjogdHJ1ZSwNCiAgICAiRGVsYXlCZXR3ZWVuUmVjb25uZWN0c01zIjogMjAwMCwNCiAgICAidXNlck5hbWUiOiAiYXBwIiwNCiAgICAicGFzc3dvcmQiOiAicGFzc3cwcmQiLA0KICAgICJzc2xDaXBoZXJTcGVjIjogbnVsbCwNCiAgICAic3NsQ2VydFJlcG9zaXRvcnkiOiBudWxsLA0KICAgICJzc2xDZXJ0UmVwb3NpdG9yeVBhc3N3b3JkIjogbnVsbCwNCiAgICAic3NsUGVlck5hbWUiOiBudWxsLA0KICAgICJjbGllbnRJZCI6ICJicm9hZGNhc3RoYW5kbGVyc2VydmNpZSINCn0=";
        public static string DefaultMqAutoReconnect => "ew0KICAgICJFbmFibGVJYm1NcUF1dG9SZWNvbm5lY3Rpb24iOiBmYWxzZSwNCiAgICAiTWF4RHVyYXRpb25JblNlY29uZHMiOiAiMTAiDQp9";
        public static string DefaultMqCustomReconnect => "ew0KICAgICJEZWxheUJldHdlZW5SZXRyaWVzTXMiOiAiMTAwMCIsDQogICAgIkVuYWJsZUN1c3RvbVJlY29ubmVjdGlvbiI6IGZhbHNlDQp9";


    }

    public static class FileExtentions {

        public static string JSON = ".json";

    }

    public static class MongoDb
    {
        public static string FindOptionsBatchSize => nameof(FindOptions.BatchSize);
        public static string FindOptionsNoCursorTimeout => nameof(FindOptions.NoCursorTimeout);
        public static string MongoClientSettingsRetryWrites => nameof(MongoClientSettings.RetryWrites);
    }

    public static class RequestHeaders
    {
        public static string ApiGatewayUserKey => "user-key";
    }
    public static class Uri
    {
        public static class Pie
        {

            public static string FetchPreFlashSoftwareOrder => "sws/api/v2/softwaremetadata";
            public static string FetchEndOfLineSoftwareOrder => "sws/api/v2/factory-endofline";
            public static string FetchFactoryVehicleCodes => "sws/api/v2/vehiclecodes";
        }
    }
}