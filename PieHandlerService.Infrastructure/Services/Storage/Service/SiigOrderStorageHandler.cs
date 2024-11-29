using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using System.Security.Cryptography;
using System.Text;

namespace PieHandlerService.Infrastructure.Services.Storage.Service;

internal class SiigOrderStorageHandler(ILogger<SiigOrderStorageHandler>? logger, IStorageOperation nasStorageOperation) : ISiigOrderStorageHandler
{
    private readonly ILogger<SiigOrderStorageHandler> _logger = logger ?? NullLogger<SiigOrderStorageHandler>.Instance;

    public async Task<FileStatusDetail> SaveEndOfLineOrder(EndOfLineOrderResponse endOfLineOrderResponse, CancellationToken cancellationToken= default)
    {
        var contentHash = GetHashString(JsonConvert.SerializeObject(endOfLineOrderResponse?.Order));
        endOfLineOrderResponse.Order.OrderHash = contentHash;
        var originHash = GetHashString(JsonConvert.SerializeObject(endOfLineOrderResponse));
        endOfLineOrderResponse.OriginHash = originHash;
        var fileName = GetFileName(endOfLineOrderResponse.MixNumber, endOfLineOrderResponse.OeIdentifier, SIIGOrderType.SIIGOrderEndOfLine.ToString());
        var filePath = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASOrderLocation) + Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASEndOfLineSubDirectory)+ 
                       Path.DirectorySeparatorChar+ endOfLineOrderResponse.MixNumber;
        var fileContent = JsonConvert.SerializeObject(endOfLineOrderResponse);
        _logger.LogInformation("Saving End of Line order file with name {FileName}", fileName);
        var fileStatusDetail = await nasStorageOperation.SaveFile(fileName, filePath, fileContent, cancellationToken);
        fileStatusDetail.FileHash = originHash;
        fileStatusDetail.ContentHash = contentHash;
        return fileStatusDetail;
    }

    public async Task<FileStatusDetail> SavePreFlashOrder(PreFlashOrderResponse preFlashOrderResponse, CancellationToken cancellationToken = default)
    {
        var contentHash = GetHashString(JsonConvert.SerializeObject(preFlashOrderResponse?.Order));
        preFlashOrderResponse.Order.OrderHash = contentHash;
        var originHash = GetHashString(JsonConvert.SerializeObject(preFlashOrderResponse));
        preFlashOrderResponse.OriginHash = originHash;
        var fileName = GetFileName(preFlashOrderResponse.MixNumber, preFlashOrderResponse.OeIdentifier, SIIGOrderType.SIIGOrderPreFlash.ToString());
        var filePath = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASOrderLocation) + Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory)
            +Path.DirectorySeparatorChar+ preFlashOrderResponse.MixNumber;
        var fileContent = JsonConvert.SerializeObject(preFlashOrderResponse);
        _logger.LogInformation("Saving PreFlash order file with name {FileName}", fileName);
        var fileStatusDetail = await nasStorageOperation.SaveFile(fileName, filePath, fileContent, cancellationToken);
        fileStatusDetail.FileHash = originHash;
        fileStatusDetail.ContentHash = contentHash;
        return fileStatusDetail;
    }

    public async Task<FileStatusDetail> SaveVehicleCodesOrder(VehicleCodesResponse vehicleCodesResponse, CancellationToken cancellationToken = default)
    {
        var contentHash = GetHashString(JsonConvert.SerializeObject(vehicleCodesResponse?.Order));
        vehicleCodesResponse.Order.OrderHash = contentHash;
        var originHash = GetHashString(JsonConvert.SerializeObject(vehicleCodesResponse));
        vehicleCodesResponse.OriginHash = originHash;
        var fileName = GetFileName(vehicleCodesResponse.MixNumber, vehicleCodesResponse.OeIdentifier, SIIGOrderType.SIIGOrderVehicleKeys.ToString());
        var filePath = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASOrderLocation) + Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASVehicleCodesSubDirectory)
            +Path.DirectorySeparatorChar + vehicleCodesResponse.MixNumber;
        var fileContent = JsonConvert.SerializeObject(vehicleCodesResponse);
        _logger.LogInformation("Saving Vehicle Object file with name {FileName}", fileName);
        var fileStatusDetail = await nasStorageOperation.SaveFile(fileName, filePath, fileContent, cancellationToken);
        fileStatusDetail.FileHash = originHash;
        fileStatusDetail.ContentHash = contentHash;
        return fileStatusDetail;
    }

    private static string GetFileName(string mixNumber, string oeId, string orderType) {
        return mixNumber + "_" + (!string.IsNullOrEmpty(oeId) && oeId.Length > 8 ? oeId.Substring(0, 8)+"_" : string.Empty) + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "_" + orderType;
    }

    private static string GetHashString(string input) {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }
}