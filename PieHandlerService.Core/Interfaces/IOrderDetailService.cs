using PieHandlerService.Core.Models;

namespace PieHandlerService.Core.Interfaces;

public interface IOrderDetailService
{
    Task<IEnumerable<SiigOrder>> FetchOrderDetails(SiigOrderQuerySpecification orderSpecification);
}