using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Linq.Expressions;

namespace PieHandlerService.Application.Services;

internal class MessageDetailService(
    IBroadcastMetadataRepository broadcastMetadataRepo,
    IPieOrderRepository? pieOrderRepo,
    IProblemDetailsManager problemDetailsManager,
    ILogger<MessageDetailService> logger)
    : IMessageDetailService
{
    private readonly IBroadcastMetadataRepository _broadcastMetadataRepo = broadcastMetadataRepo ?? throw new ArgumentNullException(nameof(broadcastMetadataRepo));
    private readonly IPieOrderRepository _pieOrderRepo = pieOrderRepo ?? throw new ArgumentNullException(nameof(pieOrderRepo));
    private readonly ILogger<MessageDetailService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));

    public async Task<IEnumerable<BroadcastContextMessage>> FetchBroadcastMessageDetails(BroadcastMessageSpecification broadcastMsgSpecification)
    {
        if (!string.IsNullOrEmpty(broadcastMsgSpecification.MixNumber) && !string.IsNullOrEmpty(broadcastMsgSpecification.OeIdentifier))
        {
            return await _broadcastMetadataRepo.FetchAll(GenerateExpressionFilterforBroadcast(broadcastMsgSpecification));
        }
        else
        {
            _logger.LogError($"Invalid MixNumber and OeIdentifier");
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1200BadRequest]);
        }

    }

    public async Task<IEnumerable<PieResponseMessage>> FetchPieMessageDetails(PieMessageSpecification pieMsgSpecification)
    {
        if (!string.IsNullOrEmpty(pieMsgSpecification.MixNumber) && !string.IsNullOrEmpty(pieMsgSpecification.OeIdentifier))
        {
            return await _pieOrderRepo.FetchAll(GenerateExpressionFilterforPieOrderMessage(pieMsgSpecification));
        }
        else
        {
            _logger.LogError($"Invalid MixNumber and OeIdentifier");
            throw new GeneralException(
                _problemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1200BadRequest]);
        }
    }

    private static Expression<Func<BroadcastMessageSpecification, bool>> GenerateExpressionFilterforBroadcast(BroadcastMessageSpecification broadcastMsgSpecification)
    {
        return broadcast => broadcast.MixNumber == broadcastMsgSpecification.MixNumber &&
                                                !string.IsNullOrEmpty(broadcastMsgSpecification.MixNumber) &&
                                     broadcast.OeIdentifier == broadcastMsgSpecification.OeIdentifier &&
                                     !string.IsNullOrEmpty(broadcastMsgSpecification.OeIdentifier);
    }

    private static Expression<Func<PieMessageSpecification, bool>> GenerateExpressionFilterforPieOrderMessage(PieMessageSpecification pieMsgSpecification)
    {
        return pieOrderMessage => pieOrderMessage.MixNumber == pieMsgSpecification.MixNumber &&
                                                !string.IsNullOrEmpty(pieMsgSpecification.MixNumber) &&
                                     pieOrderMessage.OeIdentifier == pieMsgSpecification.OeIdentifier &&
                                     !string.IsNullOrEmpty(pieMsgSpecification.OeIdentifier);
    }
}