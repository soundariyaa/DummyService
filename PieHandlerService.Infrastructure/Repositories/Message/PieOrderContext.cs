using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Repositories.Message;

internal class PieOrderContext : ContextBase
{
    private const string CollectionNamePieOrder = $"{nameof(PieOrderMetadata)}";
    public PieOrderContext(IOptions<DatabaseSettings> settings, ILogger logger) : base(logger)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        InitializeMongoDbClient(settings.Value);
        InitializeFindOptions(settings.Value.Options);
    }

    public IMongoCollection<Data.PieOrderMetadata> PieOrderMetadata => Database.GetCollection<Data.PieOrderMetadata>(CollectionNamePieOrder)
        .WithReadPreference(new ReadPreference(ReadPreferenceMode.Nearest));
}