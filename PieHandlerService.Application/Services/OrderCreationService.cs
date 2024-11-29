using AutoMapper;
using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using static PieHandlerService.Core.Constants;

namespace PieHandlerService.Application.Services;

internal class OrderCreationService(
    ISoftwareProviderFactory softwareProviderFactory,
    IProblemDetailsManager problemDetailsManager,
    IMapper mapper,
    ILogger<OrderCreationService> logger,
    ICertificateChainService certificateChainService,
    IHttpClientFactory httpClientFactory)
    : IOrderCreationService
{
    private readonly ISoftwareProviderFactory _softwareProviderFactory = softwareProviderFactory ?? throw new ArgumentNullException(nameof(softwareProviderFactory));
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    private readonly ILogger<OrderCreationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly ICertificateChainService _certificateChainService = certificateChainService ?? throw new ArgumentNullException(nameof(certificateChainService));

    public async Task<EndOfLineOrderResponse> CreateEndOfLineOrder(string? oeIdentifier, string? mixNumber, EndOfLineContext endOfLineContext,
         CancellationToken cancellationToken = default)
    {
        try
        {
            var pieSoftwareMetadataService = _softwareProviderFactory.CreateSoftwareMetadataService();
            var endOfLineOrderRequest = GetEndOfLineOrderRequest(endOfLineContext);
            endOfLineOrderRequest.MixNumber = mixNumber ?? string.Empty;
            var response = await pieSoftwareMetadataService.FetchEcuEndOfLineOrder(_httpClientFactory.CreateClient(HttpClientConfigurations.GetEndOfLineOrderHttpClient)
                , endOfLineOrderRequest);
            response.MixNumber = mixNumber ?? string.Empty;
            response.OeIdentifier = oeIdentifier ?? string.Empty;
            return response;
        }
        catch (PieSoftwareException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LogFailedCreation(ex, nameof(CreateEndOfLineOrder), endOfLineContext.GetType().Name, mixNumber, oeIdentifier);
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }

    public async Task<PreFlashOrderResponse> CreatePreFlashOrder(string? oeIdentifier, string? mixNumber, PreFlashContext preFlashContext, CancellationToken cancellationToken = default)
    {
        try
        {
            var pieSoftwareMetadataService = _softwareProviderFactory.CreateSoftwareMetadataService();
            var preFlashOrderRequest = GetPreFlashOrderRequest(preFlashContext);
            preFlashOrderRequest.MixNumber = mixNumber ?? string.Empty;
            var response = await pieSoftwareMetadataService.FetchEcuPreFlashOrder(_httpClientFactory.CreateClient(HttpClientConfigurations.GetPreFlashOrderHttpClient), preFlashOrderRequest);
            response.MixNumber = mixNumber ?? string.Empty;
            response.OeIdentifier = oeIdentifier ?? string.Empty;
            return response;
        }
        catch (PieSoftwareException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LogFailedCreation(ex, nameof(CreatePreFlashOrder), preFlashContext.GetType().Name, mixNumber, oeIdentifier);
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }

    public async Task<VehicleCodesResponse> CreateVehicleCodes(string? oeIdentifier, string? mixNumber, VehicleObjectContext vehicleObjectContext, CancellationToken cancellationToken = default)
    {
        try
        {
            var pieSoftwareMetadataService = _softwareProviderFactory.CreateSoftwareMetadataService();
            var vehicleCodesRequest = GetVehicleObjectOrderRequest(vehicleObjectContext);
            vehicleCodesRequest.MixNumber = mixNumber ?? string.Empty;
            var response = await pieSoftwareMetadataService.FetchFactoryVehicleCodes(_httpClientFactory.CreateClient(HttpClientConfigurations.GetVehicleCodesHttpClient),
                vehicleCodesRequest);
            var vehicleCodesResponse = response ?? new VehicleCodesResponse();
            vehicleCodesResponse.MixNumber = mixNumber ?? string.Empty;
            vehicleCodesResponse.OeIdentifier = oeIdentifier ?? string.Empty;
            return vehicleCodesResponse;
        }
        catch (PieSoftwareException)
        {
            throw;
        }
        catch (Exception ex)
        {
            LogFailedCreation(ex, nameof(CreateVehicleCodes), vehicleObjectContext.GetType().Name, mixNumber, oeIdentifier);
            throw new GeneralException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }

    private EndOfLineOrderRequest GetEndOfLineOrderRequest(EndOfLineContext endOfLineContext) =>
        new()
        {
            CertificateChain = _certificateChainService.FetchCertificateChain(),
            EndOfLine = _mapper.Map<EndOfLine>(endOfLineContext) ?? new EndOfLine()
        };

    private PreFlashOrderRequest GetPreFlashOrderRequest(PreFlashContext preFlashContext) =>
        new()
        {
            PreFlash = _mapper.Map<PreFlash>(preFlashContext) ?? new PreFlash(),
            CertificateChain = _certificateChainService.FetchCertificateChain()
        };

    private VehicleCodesRequest GetVehicleObjectOrderRequest(VehicleObjectContext vehicleObjectContext) =>
        new()
        {
            VehicleObject = vehicleObjectContext,
            CertificateChain = _certificateChainService.FetchCertificateChain()
        };

    private void LogFailedCreation(Exception ex, string methodName, string contextType, string? mixNumber, string? oeId) =>
        _logger.LogError(ex,
            "{MethodName} Error creating {ContextType} (MixNumber {MixNumber}, oeId: {OeId})",
            methodName, contextType, oeId, mixNumber);
}
