using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Repositories.Message;

internal class BroadcastMetadataContext : ContextBase
{
    private const string CollectionNameBroadcastMessage = $"{nameof(BroadcastMetadata)}";
    public BroadcastMetadataContext(IOptions<DatabaseSettings> settings, ILogger logger) : base(logger)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        InitializeMongoDbClient(settings.Value);
        InitializeFindOptions(settings.Value.Options);
    }

    public IMongoCollection<Data.BroadcastMetadata> BroadcastMetadata => Database.GetCollection<Data.BroadcastMetadata>(CollectionNameBroadcastMessage)
        .WithReadPreference(new ReadPreference(ReadPreferenceMode.Nearest));
}