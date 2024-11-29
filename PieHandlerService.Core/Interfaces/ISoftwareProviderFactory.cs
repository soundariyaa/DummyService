namespace PieHandlerService.Core.Interfaces;

public interface ISoftwareProviderFactory
{
    public ISoftwareMetadataService CreateSoftwareMetadataService();
}