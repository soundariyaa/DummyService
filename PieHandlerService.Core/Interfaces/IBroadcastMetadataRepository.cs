using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interface;

public interface IBroadcastMetadataRepository : IRepositoryModifier<BroadcastContextMessage>,
    IRepositoryReader<BroadcastMessageSpecification, BroadcastContextMessage>
{
}