using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using PieHandlerService.Infrastructure.Services.Storage.Service;
using PieHandlerService.Core.Models;
using System.Reflection;
using PieHandlerService.Infrastructure.Test.Utilities;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
namespace PieHandlerService.Infrastructure.UnitTest.Siig;

public sealed class StorageHandlerServiceTest
{
    private readonly Mock<ISoftwareProviderFactory> _softwareProviderFactory;
    private readonly Mock<IBroadcastMetadataRepository> _broadcastRepository;
    private readonly Mock<ISiigOrderRepository> _siigOrderRepository ;
    private readonly Mock<IHttpClientFactory> _httpClientFactory ;
    private readonly Mock<IOBChannelHandlerService> _iobChannelHandlerService;
    private readonly Mock<ICertificateChainService> _certificateChainService;
    private readonly Mock<ILogger<StorageHandlerService>> _logger;
    private readonly Mock<IProblemDetailHandler> _problemDetailsHandler;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<ISiigOrderStorageHandler> _storageSiigOrderHandler;
    private readonly Mock<IBroadcastContextStorageHandler> _storageBroadcastContextHandler;
    private readonly Mock<IMetricsService> _metricsService;
    private readonly CancellationTokenSource _cts;
    private readonly Mock<ISoftwareMetadataService> _softwareMetadataService;
    private readonly StorageHandlerService _storageHandlerService;
    private readonly HttpClient _httpClient;

    public StorageHandlerServiceTest()
    {
        _cts = new CancellationTokenSource();
        _softwareProviderFactory = new Mock<ISoftwareProviderFactory>();
        _softwareMetadataService = new Mock<ISoftwareMetadataService>();
        _siigOrderRepository = new Mock<ISiigOrderRepository>();
        _broadcastRepository = new Mock<IBroadcastMetadataRepository>();
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _iobChannelHandlerService = new Mock<IOBChannelHandlerService>();
        _certificateChainService = new Mock<ICertificateChainService>();
        _logger = new Mock<ILogger<StorageHandlerService>>();
        _mapper = new Mock<IMapper>();
        _storageSiigOrderHandler = new Mock<ISiigOrderStorageHandler>();
        _storageBroadcastContextHandler = new Mock<IBroadcastContextStorageHandler>();
        _metricsService = new Mock<IMetricsService>();
        _storageHandlerService = new StorageHandlerService(_logger.Object, _softwareProviderFactory.Object, _metricsService.Object, _mapper.Object,_certificateChainService.Object,
            _siigOrderRepository.Object, _broadcastRepository.Object, _httpClientFactory.Object, _storageSiigOrderHandler.Object, _iobChannelHandlerService.Object, _storageBroadcastContextHandler.Object);
    }

    [Fact]
    public async void Test_ProcessEndOfLineContext() {
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.EndOfLine,
            ContentHash = "asdasdasd1234213",
            OriginHash = "asdasd2343243214",
            Provider = "BroadcastHandlerService",
            Independent = false,
            FileName = "sample.json",
            IsPriority = false
        };

        var broadcastMessageVo = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.VehicleObject,
            ContentHash = "asdasdasd1234213",
            OriginHash = "asdasd2343243214",
            Provider = "BroadcastHandlerService",
            Independent = false,
            FileName = "sample.json",
            IsPriority = false
        };

        var _endOfLineOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_end-of-line-order.json");
        var eolOrderFile = JsonConvert.DeserializeObject<EndOfLineOrderFile>(_endOfLineOrder);

        var _vehicleObjectOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_vehicleobject-order.json");
        var voOrderFile = JsonConvert.DeserializeObject<VehicleCodeFile>(_vehicleObjectOrder);

        var _pieEndOfLine = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_endoflineresponse.json");
        var _eolOrderResponse = JsonConvert.DeserializeObject<EndOfLineOrderResponse>(_pieEndOfLine);

        _softwareProviderFactory.Setup(x => x.CreateSoftwareMetadataService()).Returns(_softwareMetadataService.Object);
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = mockHandler.ToHttpClient();
        _softwareMetadataService.Setup(x => x.FetchEcuEndOfLineOrder(httpClient, It.IsAny<EndOfLineOrderRequest>())).ReturnsAsync(() => _eolOrderResponse);
        _storageBroadcastContextHandler.Setup(x => x.FetchBroadcastContextForEndOfLine(broadcastMessage, _cts.Token)).ReturnsAsync(() => eolOrderFile);
        _storageBroadcastContextHandler.Setup(x => x.FetchBroadcastContextForVehicleCodes(broadcastMessageVo, _cts.Token)).ReturnsAsync(() => voOrderFile);
        _httpClientFactory.Setup( x=> x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var response = await _storageHandlerService.ProcessEndOfLineContext(broadcastMessage, broadcastMessageVo,_cts.Token);
        Assert.NotNull(response);
        Assert.NotNull((response.Order));

    }

    [Fact]
    public async void Test_ProcessPreFlashContext()
    {
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.PreFlash,
            ContentHash = "asdasdasd1234213",
            OriginHash = "asdasd2343243214",
            FileName = "sample.json",
            Provider = "BroadcastHandlerService",
            Independent = false,
            IsPriority = false
        };

        var broadcastMessageVo = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.VehicleObject,
            ContentHash = "asdasdasd1234213",
            OriginHash = "asdasd2343243214",
            FileName = "sample.json",
            Provider = "BroadcastHandlerService",
            Independent = false,
            IsPriority = false
        };

        var _preFlashOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_pre-flash-order.json");
        var orderFile = JsonConvert.DeserializeObject<PreFlashOrderFile>(_preFlashOrder);

        var _vehicleObjectOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_vehicleobject-order.json");
        var voOrderFile = JsonConvert.DeserializeObject<VehicleCodeFile>(_vehicleObjectOrder);

        var _piePreFlashResponse = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_preflashresponse.json");
        var _preFlashOrderResponse = JsonConvert.DeserializeObject<PreFlashOrderResponse>(_piePreFlashResponse);

        _softwareProviderFactory.Setup(x => x.CreateSoftwareMetadataService()).Returns(_softwareMetadataService.Object);
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = mockHandler.ToHttpClient();
        _softwareMetadataService.Setup(x => x.FetchEcuPreFlashOrder(httpClient, It.IsAny<PreFlashOrderRequest>())).ReturnsAsync(() => _preFlashOrderResponse);
        _storageBroadcastContextHandler.Setup(x => x.FetchBroadcastContextForPreFlash(broadcastMessage, _cts.Token)).ReturnsAsync(() => orderFile);
        _storageBroadcastContextHandler.Setup(x => x.FetchBroadcastContextForVehicleCodes(broadcastMessageVo, _cts.Token)).ReturnsAsync(() => voOrderFile);

        _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var response = await _storageHandlerService.ProcessPreFlashContext(broadcastMessage, broadcastMessageVo,_cts.Token);
        Assert.NotNull(response);
        Assert.True((response.Order?.LoadEcuSet?.Ecus?.Count()>0));
    }

    [Fact]
    public async void Test_ProcessVehicleCodeContext()
    {
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.VehicleObject,
            ContentHash = "asdasdasd1234213",
            OriginHash = "asdasd2343243214",
            Provider = "BroadcastHandlerService",
            Independent = false,
            FileName = "sample.json",
            IsPriority = false
        };

        var _vehicleObjectOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_vehicleobject-order.json");
        var orderFile = JsonConvert.DeserializeObject<VehicleCodeFile>(_vehicleObjectOrder);

        var _vehicleObjectResponse = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_vehicleobjectresponse.json");
        var _vehicleCodesResponse = JsonConvert.DeserializeObject<VehicleCodesResponse>(_vehicleObjectResponse);

        _softwareProviderFactory.Setup(x => x.CreateSoftwareMetadataService()).Returns(_softwareMetadataService.Object);
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = mockHandler.ToHttpClient();
        _softwareMetadataService.Setup(x => x.FetchFactoryVehicleCodes(httpClient, It.IsAny<VehicleCodesRequest>())).ReturnsAsync(() => _vehicleCodesResponse);
        _storageBroadcastContextHandler.Setup(x => x.FetchBroadcastContextForVehicleCodes(broadcastMessage, _cts.Token)).ReturnsAsync(() => orderFile);

        _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var response = await _storageHandlerService.ProcessVehicleCodeContext(broadcastMessage, _cts.Token);
        Assert.NotNull(response);
        Assert.NotNull((response.KeyManifest));
    }

}