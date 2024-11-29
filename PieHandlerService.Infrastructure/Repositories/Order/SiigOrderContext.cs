using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Repositories.Order;

internal sealed class SiigOrderContext : ContextBase
{
    private const string CollectionNameSiigOrder = $"{nameof(SiigOrder)}";
    public SiigOrderContext(IOptions<DatabaseSettings> settings, ILogger logger) : base(logger)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        InitializeMongoDbClient(settings.Value);
        InitializeFindOptions(settings.Value.Options);
    }

    public IMongoCollection<Data.SiigOrder> SiigOrder => Database.GetCollection<Data.SiigOrder>(CollectionNameSiigOrder)
        .WithReadPreference(new ReadPreference(ReadPreferenceMode.Nearest));
}