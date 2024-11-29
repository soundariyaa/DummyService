using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interface;

public interface IPieOrderRepository : IRepositoryModifier<PieResponseMessage>,
    IRepositoryReader<PieMessageSpecification, PieResponseMessage>
{
}