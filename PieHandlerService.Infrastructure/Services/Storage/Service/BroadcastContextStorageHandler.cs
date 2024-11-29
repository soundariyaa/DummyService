using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using System.Dynamic;

namespace PieHandlerService.Infrastructure.Services.Storage.Service;

internal class BroadcastContextStorageHandler(ILogger<BroadcastContextStorageHandler>? logger, IMetricsService metricsService, IProblemDetailHandler problemDetailsHandler, IStorageOperation nasStorageOperation) :
    IBroadcastContextStorageHandler
{
    private readonly IProblemDetailHandler _problemDetailsHandler = problemDetailsHandler ?? throw new ArgumentNullException(nameof(problemDetailsHandler));
    private readonly ILogger<BroadcastContextStorageHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
    private readonly IStorageOperation _nasStorageOperation = nasStorageOperation ?? throw new ArgumentNullException(nameof(nasStorageOperation));

    private static IDictionary<string, string> RequestTypeMetricsMap = LoadRequestTypeMetrics();

    public static IDictionary<string, string> LoadRequestTypeMetrics() {
        var RequestTypeMetricsMap = new Dictionary<string, string>();
        RequestTypeMetricsMap.Add(RequestType.EndOfLine.ToString(), Constants.Metrics.HandledStorageEndOfLineBroadcastMetric);
        RequestTypeMetricsMap.Add(RequestType.PreFlash.ToString(), Constants.Metrics.HandledStoragePreFlashBroadcastMetric);
        RequestTypeMetricsMap.Add(RequestType.VehicleObject.ToString(), Constants.Metrics.HandledStorageVehicleObjectBroadcastMetric);
        return RequestTypeMetricsMap;
    }

    public async Task<EndOfLineOrderFile> FetchBroadcastContextForEndOfLine(BroadcastContextMessage broadcastContextMessage, CancellationToken cancellationToken = default)
    {
        var endOfLineOrder = new EndOfLineOrderFile();
        IsDirectoryExists(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation) +
                          Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASEndOfLineSubDirectory)+Path.DirectorySeparatorChar+broadcastContextMessage.MixNumber);

        var fileStatusContent = await nasStorageOperation.FetchFileContent(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation) + 
                                                                           Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASEndOfLineSubDirectory) + Path.DirectorySeparatorChar + broadcastContextMessage.MixNumber, broadcastContextMessage.FileName, cancellationToken);
            
        RaiseFileUnavailableError(broadcastContextMessage, fileStatusContent);
        RaiseFileValidation(broadcastContextMessage, fileStatusContent);

        if (!string.IsNullOrEmpty(fileStatusContent.Content)) {
            endOfLineOrder = JsonConvert.DeserializeObject<EndOfLineOrderFile>(fileStatusContent.Content) ?? new EndOfLineOrderFile();
        }
        return endOfLineOrder;
    }

    public async Task<PreFlashOrderFile> FetchBroadcastContextForPreFlash(BroadcastContextMessage broadcastContextMessage, CancellationToken cancellationToken = default)
    {
        var preFlashOrder = new PreFlashOrderFile();
        IsDirectoryExists(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation) +
                          Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory) + Path.DirectorySeparatorChar + broadcastContextMessage.MixNumber);
        var fileStatusContent = await nasStorageOperation.FetchFileContent(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation) +
                                                                           Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory) + Path.DirectorySeparatorChar + broadcastContextMessage.MixNumber, broadcastContextMessage.FileName, cancellationToken);

        RaiseFileUnavailableError(broadcastContextMessage, fileStatusContent);
        RaiseFileValidation(broadcastContextMessage, fileStatusContent);

        if (!string.IsNullOrEmpty(fileStatusContent.Content))
        {
            preFlashOrder = JsonConvert.DeserializeObject<PreFlashOrderFile>(fileStatusContent.Content) ?? new PreFlashOrderFile();
        }
        return preFlashOrder;
    }

    public async Task<VehicleCodeFile> FetchBroadcastContextForVehicleCodes(BroadcastContextMessage broadcastContextMessage, CancellationToken cancellationToken = default)
    {
        var vehicleCode = new VehicleCodeFile();
        IsDirectoryExists(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation) +
                          Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASVehicleObjectSubDirectory) + Path.DirectorySeparatorChar + broadcastContextMessage.MixNumber);
        var fileStatusContent = await nasStorageOperation.FetchFileContent(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation) +
                                                                           Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASVehicleObjectSubDirectory) + Path.DirectorySeparatorChar + broadcastContextMessage.MixNumber, broadcastContextMessage.FileName, cancellationToken);

        RaiseFileUnavailableError(broadcastContextMessage, fileStatusContent);
        RaiseFileValidation(broadcastContextMessage, fileStatusContent);


        if (!string.IsNullOrEmpty(fileStatusContent.Content))
        {
            vehicleCode = JsonConvert.DeserializeObject<VehicleCodeFile>(fileStatusContent.Content) ?? new VehicleCodeFile();
        }
        return vehicleCode;
    }
        
    private void RaiseFileValidation(BroadcastContextMessage broadcastContextMessage, FileStatusDetail fileStatusContent) {
            
        var contentHash = string.Empty;
        var isParseSuccess = false;
        if (!string.IsNullOrEmpty(fileStatusContent.Content)) {
            if (RequestType.EndOfLine == broadcastContextMessage.RequestType)
            {
                var parsedBroadcast = JsonConvert.DeserializeObject<EndOfLineOrderFile>(fileStatusContent.Content);
                isParseSuccess = parsedBroadcast != null;
                contentHash = parsedBroadcast?.EndOfLineContext?.EndOfLineHash;
            }
            else if (RequestType.PreFlash == broadcastContextMessage.RequestType)
            {
                var parsedBroadcast = JsonConvert.DeserializeObject<PreFlashOrderFile>(fileStatusContent.Content);
                isParseSuccess = parsedBroadcast != null;
                contentHash = parsedBroadcast?.PreFlashContext?.PreFlashHash;
            }
            else if (RequestType.VehicleObject == broadcastContextMessage.RequestType)
            {
                var parsedBroadcast = JsonConvert.DeserializeObject<VehicleCodeFile>(fileStatusContent.Content);
                isParseSuccess = parsedBroadcast != null;
                contentHash = parsedBroadcast?.VehicleObjectContext?.VehicleObjectHash;
            }
        }
            
        RequestTypeMetricsMap.TryGetValue(broadcastContextMessage.RequestType.ToString(), out string? metricsType) ;

        if (!isParseSuccess)
        {
            _logger.LogError("Error parsing vehicle object context file with name {FileName} (MixNumber: {MixNumber}, OeId: {OeId})",
                broadcastContextMessage.FileName, broadcastContextMessage.MixNumber, broadcastContextMessage.OeId);

            if (!string.IsNullOrEmpty(metricsType)) { _metricsService.IncreaseStorageBroadcastCounters(metricsType, Constants.Metrics.FailureValue); }
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FileName = broadcastContextMessage?.FileName;
            throw new StorageException(
                _problemDetailsHandler.Handle(expandoObject));
        }

        if (!string.IsNullOrEmpty(contentHash) && !string.IsNullOrEmpty(broadcastContextMessage.ContentHash) && 
            !CheckHashMatch(contentHash, broadcastContextMessage.ContentHash))
        {
            _logger.LogError("Error matching the hash of the broadcast context file and broadcast context message (MixNumber: {MixNumber}, OeId: {OeId})",
                broadcastContextMessage.MixNumber, broadcastContextMessage.OeId);

            if (!string.IsNullOrEmpty(metricsType)) { _metricsService.IncreaseStorageBroadcastCounters(metricsType, Constants.Metrics.FailureValue); }
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FileName = broadcastContextMessage?.FileName;
            throw new StorageException(
                _problemDetailsHandler.Handle(expandoObject));
        }
    }

    private void RaiseFileUnavailableError(BroadcastContextMessage broadcastContextMessage, FileStatusDetail fileStatusDetail) {

        if (!fileStatusDetail.IsAvailable || string.IsNullOrEmpty(fileStatusDetail.Content))
        {
            _logger.LogError("Error finding the broadcast context file in the given location from the inbound broadcast message (MixNumber: {MixNumber}, OeId: {OeId})",
                broadcastContextMessage.MixNumber, broadcastContextMessage.OeId);
            RequestTypeMetricsMap.TryGetValue(broadcastContextMessage.RequestType.ToString(), out string? metricsType);
            if (!string.IsNullOrEmpty(metricsType)) { _metricsService.IncreaseStorageBroadcastCounters(metricsType, Constants.Metrics.FailureValue);}
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FileName = broadcastContextMessage?.FileName;
            throw new StorageException(
                _problemDetailsHandler.Handle(expandoObject));
        }
    }

    private bool CheckHashMatch(string fileHash, string messageHash) {
        return fileHash.Equals(messageHash, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsDirectoryExists(string directoryPath) {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        return Directory.Exists(directoryPath);
    }
}