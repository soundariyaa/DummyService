using MongoDB.Driver;
using Polly;

namespace PieHandlerService.Infrastructure.Repositories;

internal class RepositoryBase
{
    private const int StatusCode16500TooManyRequests = 16500;
    private const int StatusCode429RequestRateIsLarge = 429;
    private const string CommandGetMoreFailed = "Command getMore failed";
    private const string CommandUpdateFailed = "Command update failed";
    private const string CommandErrorMessage = "Request rate is large";
    private const string ExceptionMessageRequestTimedOut = "Request timed out";
    private const int MaxRetryAttempts = 10;
    private const int WaitBetweenFailures = 1000;

    private IAsyncPolicy Policy { get; }

    protected RepositoryBase() : this(Polly.Policy
        .Handle<MongoCommandException>(
            ex => ex.Code == StatusCode16500TooManyRequests ||
                  ex.Code == StatusCode429RequestRateIsLarge ||
                  ex.ErrorMessage.Contains(CommandGetMoreFailed, StringComparison.OrdinalIgnoreCase) ||
                  ex.ErrorMessage.Contains(CommandUpdateFailed, StringComparison.OrdinalIgnoreCase) ||
                  ex.ErrorMessage.Contains(CommandErrorMessage, StringComparison.OrdinalIgnoreCase) ||
                  ex.Message.Contains(CommandGetMoreFailed, StringComparison.OrdinalIgnoreCase))
        .Or<MongoWriteException>(ex => 
            ex.WriteError.Category == ServerErrorCategory.ExecutionTimeout || 
            ex.Message.Contains(ExceptionMessageRequestTimedOut, StringComparison.OrdinalIgnoreCase))
        .Or<MongoBulkWriteException>(ex => 
            ex.Message.Contains(ExceptionMessageRequestTimedOut, StringComparison.OrdinalIgnoreCase))
        .WaitAndRetryAsync(MaxRetryAttempts, x => TimeSpan.FromMilliseconds(WaitBetweenFailures)))
    { }

    protected RepositoryBase(IAsyncPolicy policy)
    {
        Policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    protected virtual async Task<T> HandleTooManyRequestWithRetries<T>(Func<Task<T>> func) where T : class
    {
        return await Policy.ExecuteAsync(func);
    }

    protected virtual async Task HandleTooManyRequestWithRetries(Func<Task> func)
    {
        await Policy.ExecuteAsync(func);
    }
}