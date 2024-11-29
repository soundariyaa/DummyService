using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Repositories.Message.Data;
using System.Linq.Expressions;
using PieHandlerService.Infrastructure.Extensions;

namespace PieHandlerService.Infrastructure.Repositories.Message;

internal class PieOrderRepository : RepositoryBase, IPieOrderRepository
{
    private readonly PieOrderContext _context;
    private readonly ILogger<PieOrderRepository> _logger;
    private readonly IMapper _mapper;
    private readonly IProblemDetailsManager _problemDetailsManager;

    public PieOrderRepository(
        IOptions<DatabaseSettings> settings,
        ILoggerFactory loggerFactory,
        IMapper mapper,
        IProblemDetailsManager problemDetailsManager)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentException(nameof(loggerFactory));
        }

        _logger = loggerFactory.CreateLogger<PieOrderRepository>();
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _problemDetailsManager =
            problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
        _context = new PieOrderContext(settings, loggerFactory.CreateLogger<PieOrderContext>());
    }

    public async Task Add(PieResponseMessage item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var pieOrder = _mapper.Map<PieOrderMetadata>(item) ??
                               throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
                await _context.PieOrderMetadata.InsertOneAsync(pieOrder, cancellationToken: cancellationToken);
                _logger.LogInformation("Added Pie response metadata with OeId: {OeId}", item.OeId);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task Delete(PieResponseMessage item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var pieOrder = _mapper.Map<PieOrderMetadata>(item) ??
                               throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
                await _context.PieOrderMetadata.InsertOneAsync(pieOrder, cancellationToken: cancellationToken);
                _logger.LogInformation("Added Pie response with mixNumber: {MixNumber}, oeId: {OeId}", item.MixNumber, item.OeId);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public Task<bool> Evaluate(Expression<Func<PieMessageSpecification, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public static Expression<Func<PieMessageSpecification, bool>> GenerateExpressionFilterForPieOrderMessage(PieMessageSpecification pieMsgSpecification)
    {
        return pieOrderMessage => pieOrderMessage.MixNumber == pieMsgSpecification.MixNumber &&
                                  !string.IsNullOrEmpty(pieMsgSpecification.MixNumber) &&
                                  pieOrderMessage.OeIdentifier == pieMsgSpecification.OeIdentifier &&
                                  !string.IsNullOrEmpty(pieMsgSpecification.OeIdentifier);
    }

    public async Task<PieResponseMessage> Fetch(Expression<Func<PieMessageSpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<PieOrderMetadata, bool>>>(filter);
            var dbResult = new PieOrderMetadata();
            await HandleTooManyRequestWithRetries(async () =>
            {
                dbResult = await _context.PieOrderMetadata
                    .Find(dbFilter, _context.FindOptions).FirstOrDefaultAsync();
            });

            return _mapper.Map<PieResponseMessage>(dbResult) ??
                   throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task<IEnumerable<PieResponseMessage>> FetchAll(Expression<Func<PieMessageSpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<PieOrderMetadata, bool>>>(filter);
            var dbResult = new List<PieOrderMetadata>();
            await HandleTooManyRequestWithRetries(async () =>
            {
                dbResult = await _context.PieOrderMetadata
                    .Find(dbFilter, _context.FindOptions).ToListAsync();
            });
            return _mapper.Map<IEnumerable<PieResponseMessage>>(dbResult) ??
                   throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task Update(PieResponseMessage item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var pieOrder = _mapper.Map<PieOrderMetadata>(item) ??
                               throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
                await _context.PieOrderMetadata.InsertOneAsync(pieOrder, cancellationToken: cancellationToken);
                _logger.LogInformation("Updated Pie response with mixNumber: {MixNumber}, oeId: {OeId}", item.MixNumber, item.OeId);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }
}