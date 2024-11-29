using MediatR;
using Microsoft.AspNetCore.Mvc;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Api.Extensions;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using ProblemDetails = PieHandlerService.Core.Models.ProblemDetails;

namespace PieHandlerService.Api.Controller;

[Route("api/message")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class MessageDetailsController(
    ISender mediator,
    IProblemDetailsManager problemDetailsManager,
    ILogger<MessageDetailsController> logger)
    : ControllerBase
{

    private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly ILogger<MessageDetailsController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("/pieordermessage")]
    [ProducesResponseType(typeof(Contracts.PieOrderMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<Contracts.PieOrderMessageResponse>> GetPieOrderMessagesByMixNumber([FromBody] Contracts.PieMessageSpecification pieMessageSpecification, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var response = await _mediator.Send(pieMessageSpecification, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is DataStoreException || ex is GeneralException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(GetPieOrderMessagesByMixNumber), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString(); ;
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(GetPieOrderMessagesByMixNumber), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(GetPieOrderMessagesByMixNumber), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }

        HttpContext.Response.StatusCode = problemDetails.Status;
        return StatusCode(problemDetails.Status, new Contracts.PieOrderMessageResponse());
    }

    [HttpPost("/broadcastmessage")]
    [ProducesResponseType(typeof(BroadcastMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseBase), StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<BroadcastMessageResponse>> GetBroadcastContextMessagesByMixNumber([FromBody] BroadcastMessageSpecification broadcastMessageSpecification, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;

        try
        {
            var response = await _mediator.Send(broadcastMessageSpecification, cancellationToken);
            HttpContext.Response.StatusCode = response.Status;
            return StatusCode(response.Status, response);
        }
        catch (Exception ex) when (ex is DataStoreException || ex is GeneralException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(GetBroadcastContextMessagesByMixNumber), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString(); ;
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
        {
            problemDetails = _problemDetailsManager.
                GenerateRequestTerminatedProblemDetails(nameof(GetBroadcastContextMessagesByMixNumber), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }
        catch (Exception ex)
        {
            problemDetails = _problemDetailsManager.
                GenerateUnhandledExceptionProblemDetails(nameof(GetBroadcastContextMessagesByMixNumber), ex, _logger);
            problemDetails.Instance = HttpContext.Request.PathAsString();
        }

        HttpContext.Response.StatusCode = problemDetails.Status;
        return StatusCode(problemDetails.Status, new BroadcastMessageResponse());
    }
}