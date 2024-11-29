using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Pie.Contracts;

public class PreFlashOrderResponse : ResponseBase
{

    public PreFlashOrderResponse() { 
        }

        public string PackageIdentity { get; set; } = string.Empty;
        public IEnumerable<Ecu> Ecus { get; set; } = new List<Ecu>();
        public string Version { get; set; } = string.Empty;
        public string KeyManifest { get; set; } = string.Empty;
        public EncryptionData EncryptionData { get; set; } = new EncryptionData();
        public string? StorageLocation { get; set; } = string.Empty;
        public string? VehicleCodesSessionKeyId { get; set; } = Constants.Factory.VehicleCodesSessionKeyId;
        public string? VbfSessionKeyId { get; set; } = Constants.Factory.VbfSessionKeyId;
}
