using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Api.Extensions;
using PieHandlerService.Core.Interfaces;
using ProblemDetails = PieHandlerService.Core.Models.ProblemDetails;

namespace PieHandlerService.Api.Controller;

[Route("api/admin")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AdminController(
    ISender mediator,
    IProblemDetailsManager problemDetailsManager,
    ILogger<AdminController> logger)
    : ControllerBase
{
    private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly ILogger<AdminController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    [HttpPost("/registerlistener")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<AdminResponse>> StartMessageListener(CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var adminRequest = new AdminRequest
            {
                RegisterQueue = true
            };
            var response = await _mediator.Send(adminRequest, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(StartMessageListener), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(StartMessageListener), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }

        HttpContext.Response.StatusCode = problemDetails.Status;
        return StatusCode(problemDetails.Status, new AdminResponse());
    }

    [HttpPost("/unregisterlistener")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<AdminResponse>> StopMessageListener(CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var adminRequest = new AdminRequest
            {
                RegisterQueue = false
            };
            var response = await _mediator.Send(adminRequest, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(StartMessageListener), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(StartMessageListener), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }

        HttpContext.Response.StatusCode = problemDetails.Status;
        return StatusCode(problemDetails.Status, new AdminResponse());
    }
}