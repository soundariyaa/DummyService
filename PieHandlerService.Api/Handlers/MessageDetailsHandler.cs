using AutoMapper;
using MediatR;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Api.Validators;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using BroadcastMessageSpecification = PieHandlerService.Api.Contracts.BroadcastMessageSpecification;
using PieMessageSpecification = PieHandlerService.Api.Contracts.PieMessageSpecification;

namespace PieHandlerService.Api.Handlers;

public class MessageDetailsHandler(
    IProblemDetailsManager problemDetailsManager,
    IMessageDetailService messageDetailService,
    IMapper mapper,
    ILogger<MessageDetailsHandler> logger)
    : IRequestHandler<BroadcastMessageSpecification, BroadcastMessageResponse>,
        IRequestHandler<PieMessageSpecification, Contracts.PieOrderMessageResponse>
{

    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILogger<MessageDetailsHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMessageDetailService _messageDetailService = messageDetailService ?? throw new ArgumentNullException(nameof(messageDetailService));


    public async Task<BroadcastMessageResponse> Handle(BroadcastMessageSpecification request, CancellationToken cancellationToken)
    {
        var validationResult = new BroadcastContextSpecificationValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            throw new Core.Exceptions.ValidationException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error900Validation],
                validationResult.Errors);
        }

        var broadcastRequestSpec = _mapper.Map<Core.Models.BroadcastMessageSpecification>(request);
        var createObjectResponse = new BroadcastMessageResponse();
        try
        {
            var orderResponse = await _messageDetailService.FetchBroadcastMessageDetails(broadcastRequestSpec);
            createObjectResponse.BroadcastContextMessages = orderResponse;
            return createObjectResponse;
        }
        catch (Exception ex) when (ex is DataStoreException || ex is GeneralException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order details {ErrorMessage} (MixNumber: {MixNumber}, OeId: {OeId}",
                ex.Message, request?.MixNumber, request?.OeIdentifier);
            throw new GeneralException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }

    public async Task<Contracts.PieOrderMessageResponse> Handle(PieMessageSpecification request, CancellationToken cancellationToken)
    {
        var validationResult = new PieOrderSpecificationValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            throw new Core.Exceptions.ValidationException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error900Validation],
                validationResult.Errors);
        }
        var pieOrderSpecification = _mapper.Map<Core.Models.PieMessageSpecification>(request);
        var createObjectResponse = new Contracts.PieOrderMessageResponse();
        try
        {
            var orderResponse = await _messageDetailService.FetchPieMessageDetails(pieOrderSpecification);
            createObjectResponse.PieResponseMessages = orderResponse;
            return createObjectResponse;
        }
        catch (Exception ex) when (ex is DataStoreException || ex is GeneralException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching order details {ErrorMessage} (MixNumber: {MixNumber}, OeId: {OeId}",
                ex.Message, request?.MixNumber, request?.OeIdentifier);
            throw new GeneralException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException]);
        }
    }
}