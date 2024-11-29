using Microsoft.Extensions.Hosting;
using PieHandlerService.Core.Interfaces;


namespace PieHandlerService.Infrastructure.Services;

public abstract class BackGroundWorkerService(IHostApplicationLifetime lifetime) : BackgroundService, IBackgroundWorkerService
{
    protected readonly IHostApplicationLifetime HostApplicationLifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));

    protected virtual async Task AwaitApplicationStarted(CancellationToken cancellationToken)
    {
        var executionStartedSource = new TaskCompletionSource();
        var cancelledRequestedSource = new TaskCompletionSource();
        await using var applicationStartedTokenRegistration = HostApplicationLifetime.ApplicationStarted.Register(() => executionStartedSource.SetResult());
        await using var cancellationTokenRegistration = cancellationToken.Register(() => cancelledRequestedSource.SetResult());
        _ = await Task.WhenAny(executionStartedSource.Task, cancelledRequestedSource.Task).ConfigureAwait(false);
    }

}