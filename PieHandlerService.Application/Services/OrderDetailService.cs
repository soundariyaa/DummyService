using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Linq.Expressions;


namespace PieHandlerService.Application.Services;

internal class OrderDetailService(
    ISiigOrderRepository siigOrderRepository,
    IProblemDetailsManager problemDetailsManager,
    ILogger<OrderDetailService> logger)
    : IOrderDetailService
{
    private readonly ISiigOrderRepository _siigOrderRepository = siigOrderRepository ?? throw new ArgumentNullException(nameof(siigOrderRepository));
    private readonly ILogger<OrderDetailService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));

    public async Task<IEnumerable<SiigOrder>> FetchOrderDetails(SiigOrderQuerySpecification orderSpecification)
    {
        var orderSpecificationRequest = GenerateExpressionFilterForSiigOrder(orderSpecification);
        try
        {
            _logger.LogInformation("Fetch siig order list from database for MixNumber {MixNumber} and OeId {OeId}",
                orderSpecification.MixNumber, orderSpecification.OeIdentifier);
            return await _siigOrderRepository.FetchAll(orderSpecificationRequest);
        }
        catch (DataStoreException)
        {
            throw;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "{ErrorMessage}", ex.Message);
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1000UnhandledException], ex);
        }
    }

    private static Expression<Func<SiigOrderQuerySpecification, bool>> GenerateExpressionFilterForSiigOrder(SiigOrderQuerySpecification siigOrderQuerySpecification) =>
        siigOrder => siigOrder.MixNumber == siigOrderQuerySpecification.MixNumber &&
                     !string.IsNullOrEmpty(siigOrderQuerySpecification.MixNumber) &&
                     siigOrder.OeIdentifier == siigOrderQuerySpecification.OeIdentifier &&
                     !string.IsNullOrEmpty(siigOrderQuerySpecification.OeIdentifier);
}