using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Linq.Expressions;
using PieHandlerService.Infrastructure.Extensions;

namespace PieHandlerService.Infrastructure.Repositories.Order;

internal class SiigOrderRepository : RepositoryBase, ISiigOrderRepository
{
    private readonly SiigOrderContext _context;
    private readonly ILogger<SiigOrderRepository> _logger;
    private readonly IMapper _mapper;
    private readonly IProblemDetailsManager _problemDetailsManager;

    public SiigOrderRepository(
        IOptions<DatabaseSettings> settings,
        ILoggerFactory loggerFactory,
        IMapper mapper,
        IProblemDetailsManager problemDetailsManager) {
        if (loggerFactory == null)
        {
            throw new ArgumentException(nameof(loggerFactory));
        }

        _logger = loggerFactory.CreateLogger<SiigOrderRepository>();
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _problemDetailsManager =
            problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
        _context = new SiigOrderContext(settings, loggerFactory.CreateLogger<SiigOrderContext>());
    }

    public Task Delete(Core.Models.SiigOrder item, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Evaluate(Expression<Func<SiigOrderQuerySpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<Data.SiigOrder, bool>>>(filter);
            var response = await HandleTooManyRequestWithRetries(async () =>
            {
                var value = await _context.SiigOrder
                    .CountDocumentsAsync(dbFilter, new CountOptions { Limit = 1 });
                return Task.FromResult(value > 0);
            });
            var result = await response;
            return result;
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task<Core.Models.SiigOrder> Fetch(Expression<Func<SiigOrderQuerySpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<Data.SiigOrder, bool>>>(filter);
            var dbResult = new Data.SiigOrder();
            await HandleTooManyRequestWithRetries(async () =>
            {
                dbResult = await _context.SiigOrder
                    .Find(dbFilter, _context.FindOptions).FirstOrDefaultAsync();
            });
            return _mapper.Map<Core.Models.SiigOrder>(dbResult) ?? throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task<IEnumerable<Core.Models.SiigOrder>> FetchAll(Expression<Func<SiigOrderQuerySpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<Data.SiigOrder, bool>>>(filter);
            var dbResult = new List<Data.SiigOrder>();
            await HandleTooManyRequestWithRetries(async () =>
            {
                dbResult = await _context.SiigOrder
                    .Find(dbFilter, _context.FindOptions).ToListAsync();
            });
            return _mapper.Map<IEnumerable<Core.Models.SiigOrder>>(dbResult) ?? throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task Update(Core.Models.SiigOrder item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var siigOrder = _mapper.Map<Data.SiigOrder>(item) ??
                                throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
                await _context.SiigOrder.InsertOneAsync(siigOrder, cancellationToken: cancellationToken);
                _logger.LogInformation("Updated siig order with mixNumber: {MixNumber}, OeId: {OeId}", item.MixNumber, item.OeIdentifier);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task Add(Core.Models.SiigOrder item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var siigOrder = _mapper.Map<Data.SiigOrder>(item) ?? 
                                throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
                await _context.SiigOrder.InsertOneAsync(siigOrder);
                _logger.LogInformation("Added siig order with mixNumber: {MixNumber}, OeId: {OeId}", item.MixNumber, item.OeIdentifier);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public static Expression<Func<SiigOrderQuerySpecification, bool>> GenerateExpressionFilterForExistingSiigOrder(SiigOrderQuerySpecification siigOrderSpecification) =>
        entity => entity.MixNumber == siigOrderSpecification.MixNumber
                  && entity.OeIdentifier == siigOrderSpecification.OeIdentifier
                  && entity.OrderType == siigOrderSpecification.OrderType;
}