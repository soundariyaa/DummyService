using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PieHandlerService.Core.Interfaces;

namespace PieHandlerService.Application.Services;

internal class AdminService(ILogger<AdminService> logger, IServiceScopeFactory serviceScopeFactory, IIBChannelHandlerService ibChannelHandlerService) : IAdminService
{
    private readonly ILogger<AdminService> _logger = logger ?? NullLogger<AdminService>.Instance;

    private readonly IIBChannelHandlerService _ibChannelHandlerService = ibChannelHandlerService ?? throw new ArgumentNullException(nameof(ibChannelHandlerService));

    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    public Task<bool> StartMessageQueueListener(CancellationToken cancellationToken = default)
    {
        var isSuccess = false;
        try
        {
            using var topLevelScope = _serviceScopeFactory.CreateScope();
            _ibChannelHandlerService.RegisterMqBroadcastContextListener(topLevelScope, cancellationToken);
            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to start the message queue listener {ErrorMessage}", ex.Message);
        }
        return Task.FromResult(isSuccess);
    }

    public Task<bool> StopMessageQueueListener()
    {
        var isSuccess = false;
        try
        {
            _ibChannelHandlerService.UnRegisterMqBroadcastContextListener();
            isSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to stop the message queue listener {ErrorMessage}", ex.Message);
        }
        return Task.FromResult(isSuccess);
    }
}