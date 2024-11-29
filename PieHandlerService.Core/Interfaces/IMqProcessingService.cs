using Microsoft.Extensions.DependencyInjection;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IMqProcessingService
{
    Task publishPieHandlerResponse(PieResponseMessage pieResponseMessage);

    void UnRegisterMqBroadcastContextListener();

    void RegisterMqBroadcastContextListener(IServiceScope serviceScope, CancellationToken cancellationToken);
}