using MediatR;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Api.Validators;
using PieHandlerService.Core.Models;
using PieHandlerService.Core.Exceptions;

namespace PieHandlerService.Api.Handlers;

public sealed class FactoryOrderHandler(
    IProblemDetailsManager problemDetailsManager,
    IOrderCreationService orderCreationService,
    ILogger<FactoryOrderHandler> logger)
    :
        IRequestHandler<EOLOrderRequest, EOLOrderResponse>,
        IRequestHandler<Contracts.PreFlashOrderRequest, Contracts.PreFlashOrderResponse>,
        IRequestHandler<Contracts.VehicleCodesRequest, Contracts.VehicleCodesResponse>
{

    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly ILogger<FactoryOrderHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOrderCreationService _orderCreationService = orderCreationService ?? throw new ArgumentNullException(nameof(orderCreationService));

    public async Task<EOLOrderResponse> Handle(EOLOrderRequest request, CancellationToken cancellationToken)
    {

        var validationResult = new EndOfLineOrderValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error900Validation],
                validationResult.Errors);
        }
        var createObjectResponse = new EOLOrderResponse();
        try
        {
            var response = await _orderCreationService.CreateEndOfLineOrder(request.OeId, request.MixNumber, request.EndOfLineContext, cancellationToken);
            createObjectResponse.MixNumber = response?.MixNumber;
            createObjectResponse.OeIdentifier = response?.OeIdentifier;
            createObjectResponse.Order = response?.Order;
            createObjectResponse.PieKeyManifest = response?.PieKeyManifest;
            return createObjectResponse;
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is GeneralException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating endofline order with exception {ErrorMessage} (MixNumber: {MixNumber}, OeId: {OeId}",
                ex.Message, request?.MixNumber, request?.OeId);
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }

    public async Task<Contracts.PreFlashOrderResponse> Handle(Contracts.PreFlashOrderRequest request, CancellationToken cancellationToken)
    {
        var validationResult = new PreFlashOrderValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error900Validation],
                validationResult.Errors);
        }
        var createObjectResponse = new Contracts.PreFlashOrderResponse();
        try
        {
            var response = await _orderCreationService.CreatePreFlashOrder(request.OeId, request.MixNumber, request.PreFlashContext, cancellationToken);
            createObjectResponse.Order = response.Order;
            createObjectResponse.MixNumber = response?.MixNumber;
            createObjectResponse.OeIdentifier = response?.OeIdentifier;
            createObjectResponse.KeyManifest = response?.KeyManifest;
            return createObjectResponse;
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is GeneralException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating preflash order with exception {ErrorMessage} (MixNumber: {MixNumber}, OeId: {OeId}",
                ex.Message, request?.MixNumber, request?.OeId);
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }


    public async Task<Contracts.VehicleCodesResponse> Handle(Contracts.VehicleCodesRequest request, CancellationToken cancellationToken)
    {

        var validationResult = new VehicleCodesValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error900Validation],
                validationResult.Errors);
        }
        var createObjectResponse = new Contracts.VehicleCodesResponse();
        try
        {
            var response = await _orderCreationService.
                CreateVehicleCodes(request.OeId, request.MixNumber, request.VehicleObjectContext, cancellationToken);
            createObjectResponse.MixNumber = response.MixNumber;
            createObjectResponse.OeIdentifier = response.OeIdentifier;
            createObjectResponse.Order = response.Order;
            createObjectResponse.KeyManifest = response.KeyManifest;
            createObjectResponse.PackageIdentity = response.PackageIdentity;
            return createObjectResponse;
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is GeneralException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle codes with exception {ErrorMessage} (MixNumber: {MixNumber}, OeId: {OeId}",
                ex.Message, request?.MixNumber, request?.OeId);
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }
}