using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using PieHandlerService.Infrastructure.Services.Storage.Service;
using PieHandlerService.Infrastructure.Test.Utilities;
using System.Reflection;


namespace PieHandlerService.Infrastructure.Test.Storage;

public class SiigOrderStorageHandlerTest
{
    private readonly SiigOrderStorageHandler _siigOrderStorageHandler;

    private readonly Mock<IStorageOperation> _nasStorageOperation;
    private readonly Mock<ILogger<SiigOrderStorageHandler>> _loggerMock;
    private readonly CancellationTokenSource _cts;

    public SiigOrderStorageHandlerTest()
    {
        _cts = new CancellationTokenSource();
        _nasStorageOperation = new Mock<IStorageOperation>();
        _loggerMock = new Mock<ILogger<SiigOrderStorageHandler>>();
        _siigOrderStorageHandler = new SiigOrderStorageHandler(_loggerMock.Object, _nasStorageOperation.Object);
    }

    [Fact]
    public async void GetFile_EndOfLinehOrder() {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASOrderLocation, "order");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory, "endofline");
        var _pieEndOfLineOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_endoflineresponse.json");
        var order = JsonConvert.DeserializeObject<Order>(_pieEndOfLineOrder);
        var endOflineResponse = new EndOfLineOrderResponse
        {
            OeIdentifier = "318b316d-405e-4df0-b272-93d155fdc120",
            MixNumber = "5036299",
            Order = order,
            PieKeyManifest = "keymanifest1"
        };
        var fileStatusDetail = new FileStatusDetail(FileStatusType.CREATED, "sample.json", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true);
        _nasStorageOperation.Setup(x => 
            x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),_cts.Token)
        ).ReturnsAsync(() => fileStatusDetail);
        var savedFileStatus = _siigOrderStorageHandler.SaveEndOfLineOrder(endOflineResponse, _cts.Token);
    }

    [Fact]
    public async void GetFile_SavePreFlashOrder()
    {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASOrderLocation, "order");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory, "preflash");
        var _piePreFlashOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_preflashresponse.json");
        var ecus = JsonConvert.DeserializeObject<EcuSet>(_piePreFlashOrder);
        var ecuSet = new EcuSet
        {
            Ecus = ecus.Ecus,
            PackageIdentity = ecus?.PackageIdentity,
        };
        var order = new Order
        {
            LoadEcuSet = ecuSet,
            Version = "1.0"
        };
        var preFlashOrderResponse = new PreFlashOrderResponse
        {
            OeIdentifier = "",
            MixNumber = "",
            Order = order,
            KeyManifest = "keymanifest1"
        };
        var fileStatusDetail = new FileStatusDetail(FileStatusType.CREATED, "sample.json", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),true);
        _nasStorageOperation.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),_cts.Token)).ReturnsAsync(() => fileStatusDetail);
        var savedFileStatus = _siigOrderStorageHandler.SavePreFlashOrder(preFlashOrderResponse, _cts.Token);
    }

    [Fact]
    public async void GetFile_SaveVehicleCodesOrder()
    {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASOrderLocation, "order");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory, "vehiclecodes");
        var _pieVehicleObjectOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_vehicleobjectresponse.json");
        var vehicleObjectResponse = JsonConvert.DeserializeObject<VehicleCodesResponse>(_pieVehicleObjectOrder);
        vehicleObjectResponse.MixNumber = "1414144";
        vehicleObjectResponse.OeIdentifier = "318b316d-405e-4df0-b272-93d155fdc120";
        var fileStatusDetail = new FileStatusDetail(FileStatusType.CREATED, "sample.json", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true);
        _nasStorageOperation.Setup(x => x.SaveFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),_cts.Token)).ReturnsAsync(() => fileStatusDetail);
        var savedFileStatus = _siigOrderStorageHandler.SaveVehicleCodesOrder(vehicleObjectResponse, _cts.Token);
    }

}