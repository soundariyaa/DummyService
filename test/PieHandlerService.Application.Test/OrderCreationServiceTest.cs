using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using PieHandlerService.Application.Services;
using PieHandlerService.Application.Test;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Reflection;

namespace PieHandlerService.Application.UnitTest;

public class OrderCreationServiceTest
{
    private readonly Mock<ISoftwareProviderFactory> _softwareProviderFactory;
    private readonly Mock<ISoftwareMetadataService> _metadataService;
    private readonly Mock<IHttpClientFactory> _httpClientFactory;
    private readonly Mock<ILogger<OrderCreationService>> _logger;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IProblemDetailsManager> _problemDetailsManager;
    private readonly Mock<ICertificateChainService> _certificateChainService;
    private readonly IOrderCreationService _orderCreationService;

    public OrderCreationServiceTest() {
        _softwareProviderFactory = new Mock<ISoftwareProviderFactory>();
        _metadataService = new Mock<ISoftwareMetadataService>();
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _logger = new Mock<ILogger<OrderCreationService>>();
        _mapper = new Mock<IMapper>();
        _problemDetailsManager = new Mock<IProblemDetailsManager>();
        _certificateChainService = new Mock<ICertificateChainService>();
        _orderCreationService = new OrderCreationService(_softwareProviderFactory.Object, _problemDetailsManager.Object, _mapper.Object, _logger.Object, 
            _certificateChainService.Object, _httpClientFactory.Object);
    }

    [Fact]
    public void Test_CreateEndOfLineOrder() {
        var _endoflineBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_end-of-line.json");
        var _pieEndOfLineOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_endoflineresponse.json");
        var endOfLine = JsonConvert.DeserializeObject<EndOfLineContext>(_endoflineBroadcastFile);
        var order = JsonConvert.DeserializeObject<Order>(_pieEndOfLineOrder);
        var endOflineResponse = new EndOfLineOrderResponse
        {
            OeIdentifier = "",
            MixNumber = "",
            Order = order,
            PieKeyManifest = "keymanifest1"
        };
        var metadataService = _softwareProviderFactory.Setup(x =>x.CreateSoftwareMetadataService()).Returns(_metadataService.Object);
        _metadataService.Setup(x => x.FetchEcuEndOfLineOrder(It.IsAny<HttpClient>(), It.IsAny<EndOfLineOrderRequest>())).Returns(Task.FromResult<EndOfLineOrderResponse>(endOflineResponse));
        var response = _orderCreationService.CreateEndOfLineOrder("1412414", "318b316c-405e-4df0-b272-93d155fdc120", endOfLine,CancellationToken.None);
        Assert.NotNull(response?.Result);
    }

    [Fact]
    public void Test_CreatePreFlashOrder()
    {
        var _preFlashBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_pre-flash.json");
        var _piePreFlashOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_preflashresponse.json");
        var _preFlashContext = JsonConvert.DeserializeObject<PreFlashContext>(_preFlashBroadcastFile);
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
        _softwareProviderFactory.Setup(x => x.CreateSoftwareMetadataService()).Returns(_metadataService.Object);
        _metadataService.Setup(x => x.FetchEcuPreFlashOrder(It.IsAny<HttpClient>(), It.IsAny<PreFlashOrderRequest>())).
            Returns(Task.FromResult<PreFlashOrderResponse>(preFlashOrderResponse));
        var response = _orderCreationService.CreatePreFlashOrder("1412414", "318b316c-405e-4df0-b272-93d155fdc120", _preFlashContext, CancellationToken.None);
        Assert.NotNull(response?.Result);
    }

    [Fact]
    public void Test_CreateVehicleCodesOrder()
    {
        var _vehicleCodesBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_vehicleobject.json");
        var _pieVehicleObjectOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_vehicleobjectresponse.json");
        var _vehicleObjectContext = JsonConvert.DeserializeObject<VehicleObjectContext>(_vehicleCodesBroadcastFile);
        var vehicleObjectResponse = JsonConvert.DeserializeObject<VehicleCodesResponse>(_pieVehicleObjectOrder);
            
        _softwareProviderFactory.Setup(x => x.CreateSoftwareMetadataService()).Returns(_metadataService.Object);
        _metadataService.Setup(x => x.FetchFactoryVehicleCodes(It.IsAny<HttpClient>(), It.IsAny<VehicleCodesRequest>())).
            Returns(Task.FromResult<VehicleCodesResponse>(vehicleObjectResponse));
        var response = _orderCreationService.CreateVehicleCodes("1412414", "318b316c-405e-4df0-b272-93d155fdc120", _vehicleObjectContext, CancellationToken.None);
        Assert.NotNull(response?.Result);
    }

}