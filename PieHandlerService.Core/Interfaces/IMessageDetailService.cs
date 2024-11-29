using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IMessageDetailService
{
    Task<IEnumerable<BroadcastContextMessage>> FetchBroadcastMessageDetails(BroadcastMessageSpecification broadcastMsgSpecification);

    Task<IEnumerable<PieResponseMessage>> FetchPieMessageDetails(PieMessageSpecification pieMsgSpecification);
}