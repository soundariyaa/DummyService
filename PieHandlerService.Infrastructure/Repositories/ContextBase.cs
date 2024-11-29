using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PieHandlerService.Core.Models;
using System.Security.Authentication;
using PieHandlerService.Infrastructure.Extensions;

namespace PieHandlerService.Infrastructure.Repositories;

internal class ContextBase
{
    protected IMongoDatabase Database;
    protected readonly ILogger Logger;

    protected ContextBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var dbSettings = new DatabaseSettings();
        var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(dbSettings.ConnectionString));
        var client = new MongoClient(mongoClientSettings);
        Database = client.GetDatabase(dbSettings.Database);
    }

    public FindOptions FindOptions { get; private set; } = new FindOptions();

    protected virtual void InitializeMongoDbClient(DatabaseSettings settings)
    {
        try
        {
            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.ConnectionString));
            mongoClientSettings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };

            if (settings.Options.TryGetValue(Constants.MongoDb.MongoClientSettingsRetryWrites, out var retryWriteResult) &&
                retryWriteResult is bool retryWrites)
            {
                mongoClientSettings.RetryWrites = retryWrites;
            }

            var client = new MongoClient(mongoClientSettings);
            Database = client.GetDatabase(settings.Database);
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            throw;
        }
    }

    protected virtual void InitializeFindOptions(IDictionary<string, object> options)
    {
        try
        {
            FindOptions = new FindOptions();
            if (options == null || !options.Any())
            {
                return;
            }

            options.TryGetValue(Constants.MongoDb.FindOptionsNoCursorTimeout, out var noCursorTimeoutValue);
            options.TryGetValue(Constants.MongoDb.FindOptionsBatchSize, out var batchSizeValue);

            if (noCursorTimeoutValue == null && batchSizeValue == null)
            {
                Logger.LogInformation($"{nameof(FindOptions)} - could not find any parameters values for " +
                                      $"{nameof(noCursorTimeoutValue)} nor {nameof(batchSizeValue)}. No changes will be applied!");
                return;
            }

            FindOptions = new FindOptions();
            if (noCursorTimeoutValue != null)
            {
                FindOptions.NoCursorTimeout = (bool)noCursorTimeoutValue;
            }

            if (batchSizeValue == null)
            {
                return;
            }
            FindOptions.BatchSize = (int)batchSizeValue;
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            throw;
        }
    }
}