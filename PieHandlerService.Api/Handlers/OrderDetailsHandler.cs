using AutoMapper;
using MediatR;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Api.Validators;
using PieHandlerService.Core.Models;
using PieHandlerService.Core.Exceptions;


namespace PieHandlerService.Api.Handlers;

public sealed class OrderDetailsHandler(
    IProblemDetailsManager problemDetailsManager,
    IOrderDetailService orderDetailService,
    IMapper mapper,
    ILogger<OrderDetailsHandler> logger)
    :
        IRequestHandler<OrderSpecificationRequest, OrderSpecificationResponse>
{

    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILogger<OrderDetailsHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IOrderDetailService _orderDetailService = orderDetailService ?? throw new ArgumentNullException(nameof(orderDetailService));


    public async Task<OrderSpecificationResponse> Handle(OrderSpecificationRequest request, CancellationToken cancellationToken)
    {

        var validationResult = new OrderSpecificationValidator().Validate(request);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error900Validation],
                validationResult.Errors);
        }
        var orderSpecification = _mapper.Map<SiigOrderQuerySpecification>(request);
        try
        {
            var orderResponse = await _orderDetailService.FetchOrderDetails(orderSpecification);
            return _mapper.Map<OrderSpecificationResponse>(orderResponse);
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