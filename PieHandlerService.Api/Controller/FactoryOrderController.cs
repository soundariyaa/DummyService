using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Api.Contracts;
using ProblemDetails = PieHandlerService.Core.Models.ProblemDetails;
using PieHandlerService.Api.Extensions;
using PieHandlerService.Core.Exceptions;


namespace PieHandlerService.Api.Controller;

[Route("api/factoryorder")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class FactoryOrderController(
    ISender mediator,
    IProblemDetailsManager problemDetailsManager,
    ILogger<FactoryOrderController> logger)
    : ControllerBase
{

    private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly ILogger<FactoryOrderController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("/endofline")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EOLOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<EOLOrderResponse>> CreateEOLFactoryOrder([FromBody] EOLOrderRequest eolOrderRequest, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var response = await _mediator.Send(eolOrderRequest, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is GeneralException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(CreateEOLFactoryOrder), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(CreateEOLFactoryOrder), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(CreateEOLFactoryOrder), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        HttpContext.Response.StatusCode = problemDetails.Status;
        return base.StatusCode(problemDetails.Status, new EOLOrderResponse());

    }

    [HttpPost("/preflashv1")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PreFlashOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<PreFlashOrderResponse>> CreatePreFlashOrder([FromBody] PreFlashOrderRequest preFlashOrderRequest, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var response = await _mediator.Send(preFlashOrderRequest, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is GeneralException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(CreatePreFlashOrder), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(CreatePreFlashOrder), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(CreatePreFlashOrder), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }

        HttpContext.Response.StatusCode = problemDetails.Status;
        return base.StatusCode(problemDetails.Status, new PreFlashOrderResponse());
    }


    [HttpPost("/vehiclecodes")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(VehicleCodesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<VehicleCodesResponse>> CreateVehicleCodes([FromBody] VehicleCodesRequest vehicleCodesRequest, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var response = await _mediator.Send(vehicleCodesRequest, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is PieSoftwareException || ex is GeneralException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(CreateVehicleCodes), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(CreateVehicleCodes), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(CreateVehicleCodes), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        HttpContext.Response.StatusCode = problemDetails.Status;
        return base.StatusCode(problemDetails.Status, new VehicleCodesResponse());
    }

}