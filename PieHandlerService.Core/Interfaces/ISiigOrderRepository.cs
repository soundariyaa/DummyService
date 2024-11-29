using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface ISiigOrderRepository : IRepositoryModifier<SiigOrder>,
    IRepositoryReader<SiigOrderQuerySpecification, SiigOrder>
{

}