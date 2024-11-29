namespace PieHandlerService.Core.Interfaces;

public interface IClaimsMappingService
{
    IDictionary<int, string> GetClaimsMapping();
}