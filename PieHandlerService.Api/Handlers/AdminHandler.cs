using MediatR;
using PieHandlerService.Api.Contracts;
using PieHandlerService.Core.Interfaces;

namespace PieHandlerService.Api.Handlers;

public sealed class AdminHandler(
    IAdminService adminService,
    ILogger<AdminHandler> logger)
    : IRequestHandler<AdminRequest, AdminResponse>
{
    private readonly ILogger<AdminHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IAdminService _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));

    public async Task<AdminResponse> Handle(AdminRequest request, CancellationToken cancellationToken) {

        _logger.LogInformation($"Administering Message Queue");
        var result = request.RegisterQueue ? await _adminService.StartMessageQueueListener(cancellationToken) :
            await _adminService.StopMessageQueueListener();

        var adminResponse = new AdminResponse
        {
            IsComplete = result,
            IsStarted = request.RegisterQueue && result
        };
        _logger.LogInformation($"Administering Message Queue Response");
        return await Task.FromResult<AdminResponse>(adminResponse);
    }


}