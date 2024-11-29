using Microsoft.Extensions.Logging;
using Moq;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using PieHandlerService.Infrastructure.Services.Storage.Service;
using PieHandlerService.Infrastructure.Test.Utilities;
using System.Reflection;


namespace PieHandlerService.Infrastructure.Test.Storage;

public class BroadcastStorageHandlerTest
{
    private readonly Mock<IMetricsService> _metricsService;
    private readonly Mock<IStorageOperation> _nasStorageOperation;
    private readonly CancellationTokenSource _cts;
    private readonly Mock<ILogger<BroadcastContextStorageHandler>> _loggerMock;
    private readonly Mock<IProblemDetailHandler> _problemDetailHandlerMock;
    private readonly BroadcastContextStorageHandler _broadcastContextStorageHandler;

    public BroadcastStorageHandlerTest() {
        _metricsService = new Mock<IMetricsService>();
        _nasStorageOperation = new Mock<IStorageOperation>();
        _cts = new CancellationTokenSource();
        _loggerMock = new Mock<ILogger<BroadcastContextStorageHandler>>();
        _problemDetailHandlerMock = new Mock<IProblemDetailHandler>();
        _broadcastContextStorageHandler = new BroadcastContextStorageHandler(_loggerMock.Object, _metricsService.Object, _problemDetailHandlerMock.Object, _nasStorageOperation.Object);
    }

    [Fact]
    public async void Test_FetchBroadcastContextForEndOfLine() {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation, "processedbroadcast");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory, "endofline");
        var _endoflineBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_end-of-line-order.json");
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.EndOfLine,
            OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            ContentHash = "c0bbb5dc90a895d16fbfec8f5dd22164362f723466b8ce3e62740763b4182549",
            Provider = "BroadcastHandlerService",
            FileName = "sample.json",
            IsPriority = false
        };
        var fileStatusDetail = new FileStatusDetail(FileStatusType.CREATED,"sample",DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),true);
        fileStatusDetail.Content = _endoflineBroadcastFile;
        _nasStorageOperation.Setup(x => x.FetchFileContent(It.IsAny<string>(), "sample.json", _cts.Token)).ReturnsAsync(() => fileStatusDetail);
        var response = _broadcastContextStorageHandler.FetchBroadcastContextForEndOfLine(broadcastMessage, _cts.Token);
        Assert.NotNull(response);
        Assert.Equal(response?.Result?.OriginHash, broadcastMessage.OriginHash);
        Assert.Equal(response?.Result?.EndOfLineContext.EndOfLineHash, broadcastMessage.ContentHash);
    }

    [Fact]
    public void Test_FetchBroadcastContextForPreFlash()
    {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation, "processedbroadcast");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory, "preflash");
        var _preFlashBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_pre-flash-order.json");
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.PreFlash,
            OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            ContentHash = "a9be90728f43c2897c6c086c90a77dd90be8d23d3f279cbce636780169a7c66b",
            FileName = "sample.json",
            Provider = "BroadcastHandlerService",
            IsPriority = false
        };
        var fileStatusDetail = new FileStatusDetail(FileStatusType.CREATED, "sample", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true);
        fileStatusDetail.Content = _preFlashBroadcastFile;
        _nasStorageOperation.Setup(x => x.FetchFileContent(It.IsAny<string>(),"sample.json",_cts.Token)).ReturnsAsync(() => fileStatusDetail);
        var response =  _broadcastContextStorageHandler.FetchBroadcastContextForPreFlash(broadcastMessage, _cts.Token);
        Assert.NotNull(response);
        Assert.Equal(response?.Result?.OriginHash, broadcastMessage.OriginHash);
        Assert.Equal(response?.Result.PreFlashContext.PreFlashHash, broadcastMessage.ContentHash);

    }

    [Fact]
    public async Task Test_FetchBroadcastContextForVehicleCodes()
    {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASBroadcastContextLocation, "processedbroadcast");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory, "vehiclecodes");
        var _vehicleObjectBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_vehicleobject-order.json");
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.VehicleObject,
            OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            ContentHash = "c97c2e60c00410dbf989de9ee79f0ff3e4f6032439d7e840af5562c6a4aa612f",
            FileName = "sample.json",
            Provider = "BroadcastHandlerService",
            Independent = true,
            IsPriority = false
        };
        var fileStatusDetail = new FileStatusDetail(FileStatusType.CREATED, "sample", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true);
        fileStatusDetail.Content = _vehicleObjectBroadcastFile;
        _nasStorageOperation.Setup(x => x.FetchFileContent(It.IsAny<string>(), "sample.json",_cts.Token)).ReturnsAsync(() => fileStatusDetail);
        var response = await _broadcastContextStorageHandler.FetchBroadcastContextForVehicleCodes(broadcastMessage, _cts.Token);
        Assert.NotNull(response);
        Assert.Equal(response.OriginHash, broadcastMessage.OriginHash);
        Assert.Equal(response.VehicleObjectContext?.VehicleObjectHash, broadcastMessage.ContentHash);
    }
}