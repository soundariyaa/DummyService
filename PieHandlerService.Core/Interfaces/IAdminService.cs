namespace PieHandlerService.Core.Interfaces;

public interface IAdminService
{

    Task<bool> StartMessageQueueListener(CancellationToken cancellationToken);

    Task<bool> StopMessageQueueListener();
}