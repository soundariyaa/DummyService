using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Extensions;
using PieHandlerService.Infrastructure.Repositories.Message.Data;
using System.Linq.Expressions;

namespace PieHandlerService.Infrastructure.Repositories.Message;

internal class BroadcastMetadataRepository : RepositoryBase, IBroadcastMetadataRepository
{
    private readonly BroadcastMetadataContext _context;
    private readonly ILogger<BroadcastMetadataRepository> _logger;
    private readonly IMapper _mapper;
    private readonly IProblemDetailsManager _problemDetailsManager;

    public BroadcastMetadataRepository(
        IOptions<DatabaseSettings> settings,
        ILoggerFactory loggerFactory,
        IMapper mapper,
        IProblemDetailsManager problemDetailsManager)
    {
        if (loggerFactory == null)
        {
            throw new ArgumentException(nameof(loggerFactory));
        }

        _logger = loggerFactory.CreateLogger<BroadcastMetadataRepository>();
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _problemDetailsManager =
            problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
        _context = new BroadcastMetadataContext(settings, loggerFactory.CreateLogger<BroadcastMetadataContext>());
    }


    public async Task Add(BroadcastContextMessage item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var broadcastMessage = _mapper.Map<Data.BroadcastMetadata>(item) ?? 
                                       throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]);
                await _context.BroadcastMetadata.InsertOneAsync(broadcastMessage, cancellationToken: cancellationToken);
                _logger.LogInformation("Added broadcast metadata with Oe identifier {OeId}", item.OeId);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Error creating broadcast metadata with exception {ErrorMessage}", ex.Message);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public async Task Delete(BroadcastContextMessage item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var broadcastMessage = _mapper.Map<Data.BroadcastMetadata>(item) ?? 
                                       throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1400DataNotFound]);
                await _context.BroadcastMetadata.DeleteOneAsync(broadcastMessage.Id, cancellationToken);
                _logger.LogInformation("Deleted broadcast metadata with Oe identifier {OeId}", item.OeId);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Error deleting broadcast metadata with exception {ErrorMessage}", ex.Message);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }

    public Task<bool> Evaluate(Expression<Func<BroadcastMessageSpecification, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public static Expression<Func<BroadcastMessageSpecification, bool>> GenerateExpressionFilterforBroadcast(BroadcastMessageSpecification broadcastMsgSpecification)
    {
        return broadcast => broadcast.MixNumber == broadcastMsgSpecification.MixNumber &&
                            !string.IsNullOrEmpty(broadcastMsgSpecification.MixNumber) &&
                            broadcast.RequestType.Equals(broadcastMsgSpecification.RequestType.ToString()) &&
                            !string.IsNullOrEmpty(broadcastMsgSpecification.RequestType.ToString()) &&
                            broadcast.OeIdentifier == broadcastMsgSpecification.OeIdentifier &&
                            !string.IsNullOrEmpty(broadcastMsgSpecification.OeIdentifier);
    }

    public static Expression<Func<BroadcastMessageSpecification, bool>> GenerateExpressionFilter(BroadcastMessageSpecification broadcastMsgSpecification)
    {
        return broadcast => broadcast.MixNumber == broadcastMsgSpecification.MixNumber &&
                            !string.IsNullOrEmpty(broadcastMsgSpecification.MixNumber) &&
                            broadcast.RequestType.Equals(broadcastMsgSpecification.RequestType.ToString()) &&
                            !string.IsNullOrEmpty(broadcastMsgSpecification.RequestType.ToString());
    }

    public async Task<BroadcastContextMessage> Fetch(Expression<Func<BroadcastMessageSpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<Data.BroadcastMetadata, bool>>>(filter);
            var dbResult = new BroadcastMetadata();
            await HandleTooManyRequestWithRetries(async () =>
            {
                dbResult = await _context.BroadcastMetadata
                    .Find(dbFilter, _context.FindOptions).SortByDescending(x => x.ModifiedUtcMs).FirstOrDefaultAsync();
            });
            return _mapper.Map<BroadcastContextMessage>(dbResult) ?? throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1400DataNotFound]);
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1400DataNotFound], ex);
        }
    }

    public async Task<IEnumerable<BroadcastContextMessage>> FetchAll(Expression<Func<BroadcastMessageSpecification, bool>> filter)
    {
        try
        {
            var dbFilter = _mapper.MapExpression<Expression<Func<Data.BroadcastMetadata, bool>>>(filter);
            var dbResult = new List<BroadcastMetadata>();
            await HandleTooManyRequestWithRetries(async () =>
            {
                dbResult = await _context.BroadcastMetadata
                    .Find(dbFilter, _context.FindOptions).SortByDescending(x => x.ModifiedUtcMs).ToListAsync();
            });
            return _mapper.Map<IEnumerable<BroadcastContextMessage>>(dbResult) ?? throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1400DataNotFound]);
        }
        catch (MongoException ex)
        {
            _logger.LogException(ex);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1400DataNotFound], ex);
        }
    }


    public async Task Update(BroadcastContextMessage item, CancellationToken cancellationToken)
    {
        try
        {
            await HandleTooManyRequestWithRetries(async () =>
            {
                var broadcastMessage = _mapper.Map<Data.BroadcastMetadata>(item) ??
                                       throw new DataStoreException(_problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore]); ;
                await _context.BroadcastMetadata.InsertOneAsync(broadcastMessage, cancellationToken: cancellationToken);
                _logger.LogInformation("Updated broadcast metadata with Oe identifier {OeId}", item.OeId);
            });
        }
        catch (MongoException ex)
        {
            _logger.LogError(ex, "Error updating broadcast metadata with exception {ErrorMessage}", ex.Message);
            throw new DataStoreException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error400DataStore], ex);
        }
    }
}