using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Api.Contracts;
using ProblemDetails = PieHandlerService.Core.Models.ProblemDetails;
using PieHandlerService.Api.Extensions;
using PieHandlerService.Core.Exceptions;


namespace PieHandlerService.Api.Controller;

[Route("api/orderdetail")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class OrderDetailsController(
    ISender? mediator,
    IProblemDetailsManager problemDetailsManager,
    ILogger<OrderDetailsController> logger)
    : ControllerBase
{

    private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly ILogger<OrderDetailsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(EOLOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<OrderSpecificationResponse>> FetchOrderDetails([FromBody] OrderSpecificationRequest orderSpecificationRequest, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var response = await _mediator.Send(orderSpecificationRequest, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is DataStoreException || ex is GeneralException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(FetchOrderDetails), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString(); ;
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(FetchOrderDetails), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(FetchOrderDetails), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }

        HttpContext.Response.StatusCode = problemDetails.Status;
        return StatusCode(problemDetails.Status, new EOLOrderResponse());
    }


}