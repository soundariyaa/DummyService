using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Repositories.Message;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using System.Dynamic;
using PieHandlerService.Infrastructure.Repositories.Order;
using static PieHandlerService.Core.Constants;

namespace PieHandlerService.Infrastructure.Services.Storage.Service;

internal sealed class StorageHandlerService(
    ILogger<StorageHandlerService> logger,
    ISoftwareProviderFactory softwareProviderFactory,
    IMetricsService metricsService,
    IMapper mapper,
    ICertificateChainService certificateChainService,
    ISiigOrderRepository siigOrderRepository,
    IBroadcastMetadataRepository broadcastMetadataRepo,
    IHttpClientFactory httpClientFactory,
    ISiigOrderStorageHandler storageSiigOrderHandler,
    IOBChannelHandlerService iobChannelHandlerService,
    IBroadcastContextStorageHandler storageBroadcastContextHandler
) : IStorageHandlerService
{
    private readonly ISoftwareProviderFactory _softwareProviderFactory = softwareProviderFactory ?? throw new ArgumentNullException(nameof(softwareProviderFactory));
    private readonly ISiigOrderRepository _siigOrderRepository = siigOrderRepository ?? throw new ArgumentNullException(nameof(siigOrderRepository));
    private readonly IBroadcastMetadataRepository? _broadcastMetadataRepo = broadcastMetadataRepo ?? throw new ArgumentNullException(nameof(broadcastMetadataRepo));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    private readonly IOBChannelHandlerService _iobChannelHandlerService = iobChannelHandlerService ?? throw new ArgumentNullException(nameof(iobChannelHandlerService));

    private readonly ICertificateChainService _certificateChainService = certificateChainService ?? throw new ArgumentNullException(nameof(certificateChainService));
    private readonly ILogger<StorageHandlerService> _logger = logger ?? NullLogger<StorageHandlerService>.Instance;
  
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ISiigOrderStorageHandler _storageSiigOrderHandler = storageSiigOrderHandler ?? throw new ArgumentNullException(nameof(storageSiigOrderHandler));

    private readonly IBroadcastContextStorageHandler _storageBroadcastContextHandler = storageBroadcastContextHandler ?? throw new ArgumentNullException(nameof(storageSiigOrderHandler));

    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));

    private const string ClassName = nameof(StorageHandlerService);

    public async Task CheckAndInitiateEOL(BroadcastContextMessage message, CancellationToken cancellationToken, RequestType requestType) 
    {
        var queryRequestType = requestType == RequestType.EndOfLine ? RequestType.VehicleObject : RequestType.EndOfLine;
        var broadcastSpec = new BroadcastMessageSpecification(message.MixNumber, queryRequestType);
        var existingRecords = await _broadcastMetadataRepo.FetchAll(BroadcastMetadataRepository.GenerateExpressionFilter(broadcastSpec));

        if (existingRecords.Any())
        {
            var eolRequest = requestType == RequestType.EndOfLine ? message : existingRecords.First();
            var vehicleObjectRequest = requestType == RequestType.VehicleObject ? message : existingRecords.First();
            _logger.LogInformation("Handling requestType {RequestTypeProcess}: Found a {RequestType} for MixNumber: {MixNumber} (OeId: {OeId}). Will proceed with StartEndOfLineProcess",
                message.RequestType, queryRequestType, message.MixNumber, message.OeId);
            await StartEndOfLineProcess(eolRequest, vehicleObjectRequest, cancellationToken);
        }
        else
        {
            _logger.LogWarning("PIE Factory EOL API call cannot be made for (MixNumber: {MixNumber}, OeId: {OeId}) due to unavailability of entity {entity}",
                message?.MixNumber, message?.OeId,queryRequestType);
        }
    }

    public async Task CheckAndInitiatePreFlash(BroadcastContextMessage message, CancellationToken cancellationToken, RequestType requestType) 
    {
        var queryRequestType = requestType == RequestType.PreFlash ? RequestType.VehicleObject : RequestType.PreFlash;
        var broadcastSpec = new BroadcastMessageSpecification(message.MixNumber, queryRequestType);
        var existingRecords = await _broadcastMetadataRepo.FetchAll(BroadcastMetadataRepository.GenerateExpressionFilter(broadcastSpec));

        if (existingRecords.Any())
        {
            var preFlashRequest = requestType == RequestType.PreFlash ? message : existingRecords.First();
            var vehicleObjectRequest = requestType == RequestType.VehicleObject ? message : existingRecords.First();

            _logger.LogInformation("Handling requestType {RequestTypeProcess}: Found a {RequestType} for MixNumber: {MixNumber} (OeId: {OeId}). Will proceed with StartPreFlashProcess",
                    message.RequestType, queryRequestType, message.MixNumber, message.OeId);

            await StartPreFlashOrderGeneration(preFlashRequest, vehicleObjectRequest, cancellationToken);
        }
        else
        {
            _logger.LogWarning("PIE Factory PreFlash API call cannot be made for (MixNumber: {MixNumber}, OeId: {OeId}) due to unavailability of entity {entity}",
                message?.MixNumber, message?.OeId,queryRequestType);
        }
    }


    public async Task Handle(BroadcastContextMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{ClassName}.{MethodName} requestType {RequestTypeProcess} for MixNumber: {MixNumber} and OE Identifier: {OeId}",
            ClassName, nameof(Handle), message.RequestType, message.MixNumber, message.OeId);

        switch (message.RequestType)
        {
            case RequestType.EndOfLine:
                {
                    await CheckAndInitiateEOL(message, cancellationToken,message.RequestType);
                    break;
                }

            case RequestType.PreFlash:
                {
                    await CheckAndInitiatePreFlash(message, cancellationToken, message.RequestType);
                    break;
                }

            case RequestType.VehicleObject:
                {
                    if (message.Independent ?? true) {

                        await StartVehicleCodeProcess(message, cancellationToken);
                    }

                    await CheckAndInitiateEOL(message, cancellationToken, message.RequestType);

                    await CheckAndInitiatePreFlash(message, cancellationToken, message.RequestType);

                    break;
                }
        }

        _logger.LogInformation("{ClassName}.{MethodName} Completed processing requestType {RequestTypeProcess} for {FileName}", ClassName, nameof(Handle), message?.RequestType, message?.FileName);
    }

    private async Task StartPreFlashOrderGeneration(BroadcastContextMessage preFlashBroadcast, BroadcastContextMessage vehicleObjectBroadcastData, CancellationToken cancellationToken) {


        var existingPreFlashOrder = await _siigOrderRepository.FetchAll(
    SiigOrderRepository.GenerateExpressionFilterForExistingSiigOrder(
        new SiigOrderQuerySpecification(preFlashBroadcast.MixNumber, preFlashBroadcast.OeId, SIIGOrderType.SIIGOrderPreFlash)));
        if (existingPreFlashOrder.Any())
        {
            _logger.LogInformation("SIIGOrderPreFlash already exists for MixNumber: {MixNumber}, OeId: {OeId}", preFlashBroadcast.MixNumber, preFlashBroadcast.OeId);
            return;
        }

        var isSuccess = false;
        try
        {
            var preFlashResponse = await ProcessPreFlashContext(preFlashBroadcast, vehicleObjectBroadcastData, cancellationToken);

            if (preFlashResponse != null) {
                preFlashResponse.MixNumber = preFlashBroadcast.MixNumber;
                preFlashResponse.OeIdentifier = preFlashBroadcast.OeId;

                var fileStatus = await _storageSiigOrderHandler.SavePreFlashOrder(preFlashResponse, cancellationToken);
                if (fileStatus.IsAvailable)
                {
                    isSuccess = true;
                    PersistOrderResponse(JsonConvert.SerializeObject(preFlashResponse), SIIGOrderType.SIIGOrderPreFlash, preFlashBroadcast?.OeId, preFlashBroadcast?.MixNumber, cancellationToken);

                    _logger.LogInformation("Saved PreFlash Order (MixNumber: {MixNumber}, OeId: {OeId})", preFlashBroadcast?.MixNumber, preFlashBroadcast?.OeId);
                    if (preFlashBroadcast != null) { await WriteToPieResponseChannel(preFlashBroadcast, fileStatus, cancellationToken); }
                }
            }
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is StorageException)
        {
            _logger.LogError(ex, "Error processing the PreFlash order file (MixNumber: {MixNumber}, OeId: {OeId})", preFlashBroadcast?.MixNumber, preFlashBroadcast?.OeId);
        }
        catch (Exception ex)
        {
            isSuccess = false;
            _logger.LogError(ex, "Error saving PreFlash order file (MixNumber: {MixNumber}, OeId: {OeId})", preFlashBroadcast?.MixNumber, preFlashBroadcast?.OeId);
        }
        finally
        {
            _metricsService.IncreaseStorageSiigOrderCounters(
                Constants.Metrics.HandledStoragePreFlashOrderMetric,
                isSuccess ? Constants.Metrics.SuccessValue : Constants.Metrics.FailureValue);
        }
    }

    private async Task StartVehicleCodeProcess(BroadcastContextMessage vehicleObjectBroadcastData, CancellationToken cancellationToken) {

        var existingVehicleCodes = await _siigOrderRepository.FetchAll(
                    SiigOrderRepository.GenerateExpressionFilterForExistingSiigOrder(
                        new SiigOrderQuerySpecification(vehicleObjectBroadcastData.MixNumber, vehicleObjectBroadcastData.OeId, SIIGOrderType.SIIGOrderVehicleKeys)));
        if (existingVehicleCodes.Any())
        {
            _logger.LogInformation("SIIGOrderVehicleCodes already exists for MixNumber: {MixNumber}, OeId: {OeId}", vehicleObjectBroadcastData.MixNumber, vehicleObjectBroadcastData.OeId);
            return;
        }


        var isSuccess = false;
        try
        {
            var vehicleCodeResponse = await ProcessVehicleCodeContext(vehicleObjectBroadcastData, cancellationToken);
            if (vehicleCodeResponse != null) {
                vehicleCodeResponse.MixNumber = vehicleObjectBroadcastData.MixNumber;
                vehicleCodeResponse.OeIdentifier = vehicleObjectBroadcastData.OeId;

                var fileStatus = await _storageSiigOrderHandler.SaveVehicleCodesOrder(vehicleCodeResponse, cancellationToken);
                if (vehicleObjectBroadcastData != null && fileStatus.IsAvailable)
                {
                    isSuccess = true;
                    PersistOrderResponse(JsonConvert.SerializeObject(vehicleCodeResponse), SIIGOrderType.SIIGOrderVehicleKeys, vehicleObjectBroadcastData?.OeId, vehicleObjectBroadcastData?.MixNumber, cancellationToken);
                    _logger.LogInformation("Saved vehicle codes Order (MixNumber: {MixNumber}, OeId: {OeId})", vehicleObjectBroadcastData?.MixNumber, vehicleObjectBroadcastData?.OeId);
                    if (vehicleObjectBroadcastData != null) { await WriteToPieResponseChannel(vehicleObjectBroadcastData, fileStatus, cancellationToken); }
                }
            }
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is StorageException)
        {
            _logger.LogError(ex, "Error processing the vehicle codes order file (MixNumber: {MixNumber}, OeId: {OeId})", vehicleObjectBroadcastData?.MixNumber, vehicleObjectBroadcastData?.OeId);
        }
        catch (Exception ex)
        {
            isSuccess = false;
            _logger.LogError(ex, "Error saving the vehicle codes order file (MixNumber: {MixNumber}, OeId: {OeId})", vehicleObjectBroadcastData?.MixNumber, vehicleObjectBroadcastData?.OeId);
        }
        finally
        {
            _metricsService.IncreaseStorageSiigOrderCounters(
                Constants.Metrics.HandledStorageVehicleObjectOrderMetric,
                isSuccess ? Constants.Metrics.SuccessValue : Constants.Metrics.FailureValue);
        }
    }

    private async Task StartEndOfLineProcess(BroadcastContextMessage endOfLineBroadcastData, BroadcastContextMessage vehicleObjectBroadcastData, CancellationToken cancellationToken) {
        var existingEndOfLineOrder = await _siigOrderRepository.FetchAll(
            SiigOrderRepository.GenerateExpressionFilterForExistingSiigOrder(
                new SiigOrderQuerySpecification(endOfLineBroadcastData.MixNumber, endOfLineBroadcastData.OeId, SIIGOrderType.SIIGOrderEndOfLine)));
        if (existingEndOfLineOrder.Any())
        {
            _logger.LogInformation("SIIGOrderEndOfLine already exists for MixNumber: {MixNumber}, OeId: {OeId}", endOfLineBroadcastData.MixNumber, endOfLineBroadcastData.OeId);
            return;
        }
        var isEolSuccess = false;
        try
        {
            var endOfLineResponse = await ProcessEndOfLineContext(endOfLineBroadcastData, vehicleObjectBroadcastData, cancellationToken);
            if (endOfLineResponse != null) {
                endOfLineResponse.MixNumber = endOfLineBroadcastData.MixNumber;
                endOfLineResponse.OeIdentifier = endOfLineBroadcastData.OeId;
                var fileStatus = await _storageSiigOrderHandler.SaveEndOfLineOrder(endOfLineResponse, cancellationToken);
                if (fileStatus.IsAvailable)
                {
                    isEolSuccess = true;
                    PersistOrderResponse(JsonConvert.SerializeObject(endOfLineResponse), SIIGOrderType.SIIGOrderEndOfLine, endOfLineBroadcastData?.OeId, endOfLineBroadcastData?.MixNumber, cancellationToken);
                    _logger.LogInformation("Saved EOL Order (MixNumber: {MixNumber}, OeId: {OeId})", endOfLineBroadcastData?.MixNumber, endOfLineBroadcastData?.OeId);

                    if (endOfLineBroadcastData != null) { await WriteToPieResponseChannel(endOfLineBroadcastData, fileStatus, cancellationToken); }
                }
            }
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is StorageException)
        {
            _logger.LogError(ex, "Error processing the EOL order file (MixNumber: {MixNumber}, OeId: {OeId})", endOfLineBroadcastData?.MixNumber, endOfLineBroadcastData?.OeId);
        }
        catch (Exception ex)
        {
            isEolSuccess = false;
            _logger.LogError(ex, "Error saving the EOL order file (MixNumber: {MixNumber}, OeId: {OeId})", endOfLineBroadcastData?.MixNumber, endOfLineBroadcastData?.OeId);
        }
        finally
        {
            _metricsService.IncreaseStorageSiigOrderCounters(
                Constants.Metrics.HandledStorageEndOfLineOrderMetric,
                isEolSuccess ? Constants.Metrics.SuccessValue : Constants.Metrics.FailureValue);
        }
    }

    private Task WriteToPieResponseChannel(BroadcastContextMessage message, FileStatusDetail fileStatus, CancellationToken cancellationToken)
    {
        return _iobChannelHandlerService.HandleMessage(new PieResponseMessage
        {
            OeId = message.OeId!,
            MixNumber = message.MixNumber,
            CreatedUtcMs = fileStatus.CreatedUtcMs,
            ModifiedUtcMs = fileStatus.CreatedUtcMs,
            Status = OrderStatus.Available,
            OriginHash = fileStatus.FileHash,
            ContentHash = fileStatus.ContentHash,
            FileName = fileStatus.FileName,
            IsPriority = message.IsPriority,
            RequestType = TransformToOrderType(message.RequestType),
        }, cancellationToken);
    }

    private string TransformToOrderType(RequestType messageRequestType)
    {
        var orderType = messageRequestType switch
        {
            RequestType.EndOfLine => SIIGOrderType.SIIGOrderEndOfLine,
            RequestType.PreFlash => SIIGOrderType.SIIGOrderPreFlash,
            RequestType.VehicleObject => SIIGOrderType.SIIGOrderVehicleKeys,
            _ => throw new ArgumentOutOfRangeException(nameof(messageRequestType), messageRequestType, null),
        };
        return orderType.ToString();
    }

    public async Task<Core.Models.EndOfLineOrderResponse?> ProcessEndOfLineContext(BroadcastContextMessage endOfLineBroadCast, BroadcastContextMessage vehicleObjectBroadcast, CancellationToken cancellationToken)
    {
        var pieSoftwareMetadataService = _softwareProviderFactory.CreateSoftwareMetadataService();
        try
        {
            var endOfLineContextFile = await _storageBroadcastContextHandler.FetchBroadcastContextForEndOfLine(endOfLineBroadCast, cancellationToken);
            var vehicleObjectFile = await _storageBroadcastContextHandler.FetchBroadcastContextForVehicleCodes(vehicleObjectBroadcast, cancellationToken);
            var endOfLineRequest = GetEndOfLineOrderRequest(endOfLineContextFile.EndOfLineContext, vehicleObjectFile.VehicleObjectContext, endOfLineContextFile.MixNumber);
            return await pieSoftwareMetadataService.FetchEcuEndOfLineOrder(_httpClientFactory.CreateClient(HttpClientConfigurations.GetEndOfLineOrderHttpClient), endOfLineRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing EOL broadcast context file with name {FileName} (MixNumber: {MixNumber}, OeId: {OeId})",
                endOfLineBroadCast?.FileName, endOfLineBroadCast?.MixNumber, endOfLineBroadCast?.OeId);
            return null;
        }
        
    }

    public async Task<Core.Models.PreFlashOrderResponse?> ProcessPreFlashContext(BroadcastContextMessage preFlashBroadcast, BroadcastContextMessage vehicleObjectBroadcast, CancellationToken cancellationToken)
    {
        var pieSoftwareMetadataService = _softwareProviderFactory.CreateSoftwareMetadataService();
        try
        {
            var preFlashContextFile = await _storageBroadcastContextHandler.FetchBroadcastContextForPreFlash(preFlashBroadcast, cancellationToken);
            var vehicleObjectFile = await _storageBroadcastContextHandler.FetchBroadcastContextForVehicleCodes(vehicleObjectBroadcast, cancellationToken);
            var preFlashRequest = GetPreFlashOrderRequest(preFlashContextFile.PreFlashContext, vehicleObjectFile.VehicleObjectContext, preFlashContextFile.MixNumber);
            return await pieSoftwareMetadataService.FetchEcuPreFlashOrder(_httpClientFactory.CreateClient(HttpClientConfigurations.GetEndOfLineOrderHttpClient), preFlashRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PreFlash broadcast context file with name {FileName} (MixNumber: {MixNumber}, OeId: {OeId})",
                preFlashBroadcast?.FileName, preFlashBroadcast?.MixNumber, preFlashBroadcast?.OeId);
            return null;
        }
    }

    public async Task<Core.Models.VehicleCodesResponse?> ProcessVehicleCodeContext(BroadcastContextMessage broadcastContextMessage, CancellationToken cancellationToken)
    {
        var pieSoftwareMetadataService = _softwareProviderFactory.CreateSoftwareMetadataService();
        try
        {
            var vehicleCodesContextFile = await _storageBroadcastContextHandler.FetchBroadcastContextForVehicleCodes(broadcastContextMessage, cancellationToken);

            var vehicleObjectRequest = GetVehicleObjectOrderRequest(vehicleCodesContextFile.VehicleObjectContext, vehicleCodesContextFile.MixNumber);
            return await pieSoftwareMetadataService.FetchFactoryVehicleCodes(_httpClientFactory.CreateClient(HttpClientConfigurations.GetEndOfLineOrderHttpClient)
                , vehicleObjectRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Vehicle Object broadcast context file with name {FileName} (MixNumber: {MixNumber}, OeId: {OeId})",
                broadcastContextMessage?.FileName, broadcastContextMessage?.MixNumber, broadcastContextMessage?.OeId);
            return null;
        }
    }

    public void PersistOrderResponse(string? orderResponseJsonString, SIIGOrderType requestType, string? oeIdentifier, string? mixNumber, CancellationToken cancellationToken)
    {
        var siigOrder = new SiigOrder();
        siigOrder.MixNumber = mixNumber;
        siigOrder.OeIdentifier = oeIdentifier;
        siigOrder.OrderResponse = orderResponseJsonString;
        siigOrder.OrderStatus = OrderStatus.Available;
        siigOrder.OrderType = requestType;
        siigOrder.ModifiedUtcTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        siigOrder.CreatedUtcTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        _siigOrderRepository.Add(siigOrder, cancellationToken);
    }

    private EndOfLineOrderRequest GetEndOfLineOrderRequest(EndOfLineContext endOfLineContext, VehicleObjectContext vehicleObjectContext, string? mixNumber)
    {
        var softwareMetadataRequest = new EndOfLineOrderRequest();
        vehicleObjectContext.CurrentEcus.AsParallel().ForAll(x => {
            int.TryParse(x.EcuAddress, out int ecuAddressInt);
            var hexAddress = ecuAddressInt.ToString("X");
            x.EcuAddress = hexAddress;
        });
        softwareMetadataRequest.CertificateChain = _certificateChainService.FetchCertificateChain();
        softwareMetadataRequest.EndOfLine = _mapper.Map<EndOfLine>(endOfLineContext) ?? new EndOfLine();
        softwareMetadataRequest.VehicleObject = vehicleObjectContext;
        softwareMetadataRequest.VehicleObject.Vehicle.PackageIdentity = vehicleObjectContext?.Vehicle?.PackageIdentity ?? Guid.NewGuid().ToString();
        softwareMetadataRequest.MixNumber = mixNumber ?? string.Empty;
        return softwareMetadataRequest;
    }

    private PreFlashOrderRequest GetPreFlashOrderRequest(PreFlashContext preFlashContext, VehicleObjectContext vehicleObjectContext, string? mixNumber)
    {
        var softwareMetadataRequest = new PreFlashOrderRequest();
        vehicleObjectContext.CurrentEcus.AsParallel().ForAll(x => {
            int.TryParse(x.EcuAddress, out int ecuAddressInt);
            var hexAddress = ecuAddressInt.ToString("X");
            x.EcuAddress = hexAddress;
        });
        softwareMetadataRequest.MixNumber = mixNumber ?? string.Empty;
        softwareMetadataRequest.PreFlash = _mapper.Map<PreFlash>(preFlashContext) ?? new PreFlash();
        softwareMetadataRequest.VehicleObject = vehicleObjectContext;
        softwareMetadataRequest.VehicleObject.Vehicle.PackageIdentity = vehicleObjectContext?.Vehicle?.PackageIdentity ?? Guid.NewGuid().ToString();
        softwareMetadataRequest.CertificateChain = _certificateChainService.FetchCertificateChain();
        return softwareMetadataRequest;
    }

    private VehicleCodesRequest GetVehicleObjectOrderRequest(VehicleObjectContext vehicleObjectContext, string? mixNumber)
    {
        var vehicleCodesRequest = new VehicleCodesRequest();
        vehicleCodesRequest.MixNumber = mixNumber ?? string.Empty;
        vehicleObjectContext.CurrentEcus.AsParallel().ForAll(x => {
            int.TryParse(x.EcuAddress, out int ecuAddressInt);
            var hexAddress = ecuAddressInt.ToString("X");
            x.EcuAddress = hexAddress;
        });
        vehicleCodesRequest.VehicleObject = vehicleObjectContext;
        vehicleCodesRequest.VehicleObject.Vehicle.PackageIdentity = vehicleObjectContext?.Vehicle?.PackageIdentity ?? Guid.NewGuid().ToString();
        vehicleCodesRequest.CertificateChain = _certificateChainService.FetchCertificateChain();
        return vehicleCodesRequest;
    }
}