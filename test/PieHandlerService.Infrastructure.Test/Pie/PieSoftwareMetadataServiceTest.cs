using AutoMapper;
using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Pie;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using Moq;
using System.Net.Http.Json;
using System.Net;
using System.Reflection;
using PieHandlerService.Infrastructure.Test.Utilities;
using PieHandlerService.Core.Models;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;


namespace PieHandlerService.Infrastructure.UnitTest.Pie;

public class PieSoftwareMetadataServiceTest
{
    private readonly ISoftwareMetadataService _pieSoftwareMetadataService;
    private readonly Mock<ILogger<PieSoftwareMetadataService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly HttpClient _httpClient;
    private readonly Mock<IMetricsService> _metricsService;
    private readonly Mock<IStorageOperation> _nasStorageOperation;
    private readonly Mock<IVbfStorageHandler> _diskFileHandlerMock;
    private readonly Mock<IProblemDetailHandler> _problemDetailsHandler;
    private const string Uri = "https://pie-software.com";

    public PieSoftwareMetadataServiceTest() {
        _loggerMock = new Mock<ILogger<PieSoftwareMetadataService>>();
        _mapperMock = new Mock<IMapper>();
        _metricsService = new Mock<IMetricsService>();
        _problemDetailsHandler = new Mock<IProblemDetailHandler>();
        _diskFileHandlerMock = new Mock<IVbfStorageHandler>();
        _nasStorageOperation = new Mock<IStorageOperation>();
        _pieSoftwareMetadataService = new PieSoftwareMetadataService(_mapperMock.Object,_loggerMock.Object,_metricsService.Object, _diskFileHandlerMock.Object, _nasStorageOperation.Object, _problemDetailsHandler.Object);
    }


    [Fact]
    public void Test_FetchPieEndOfLineOrder() {
        var _endoflineBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_end-of-line.json");
        var _pieEndOfLineOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_endoflineresponse.json");
        var order = JsonConvert.DeserializeObject<Order>(_pieEndOfLineOrder);
        var endOfLine = JsonConvert.DeserializeObject<EndOfLine>(_endoflineBroadcastFile);

        var endOflineResponse = new Services.Pie.Contracts.EndOfLineOrderResponse
        {
            Order = order,
            PieKeyManifest = "keymanifest1"
        };
        var endOfLineRequest = new PieHandlerService.Core.Models.EndOfLineOrderRequest {
            CertificateChain = ["asdasd"],
            MixNumber = "12323123",
            EndOfLine = endOfLine
        };
        var mockHandler = new MockHttpMessageHandler();
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create<Services.Pie.Contracts.EndOfLineOrderResponse>(endOflineResponse)
        };

        var mockedRequest = mockHandler.When(HttpMethod.Post, Constants.Uri.Pie.FetchEndOfLineSoftwareOrder).Respond(mockResponse.StatusCode, mockResponse.Content);
        var httpClient = mockHandler.ToHttpClient();
        var response = _pieSoftwareMetadataService.FetchEcuEndOfLineOrder(httpClient, endOfLineRequest);
    }

    [Fact]
    public void Test_FetchPiePreFlashOrder()
    {
        var _preFlashBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_pre-flash.json");
        var _piePreFlashOrder = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_preflashresponse.json");
        var ecus = JsonConvert.DeserializeObject<EcuSet>(_piePreFlashOrder);
        var preFlash = JsonConvert.DeserializeObject<PreFlash>(_preFlashBroadcastFile);

        var preFlashResponse = new Services.Pie.Contracts.PreFlashOrderResponse
        {
            Ecus = ecus.Ecus,
            PackageIdentity = ecus.PackageIdentity,
            Version = "keymanifest1",
            KeyManifest = "keymanifest"
        };
        var PreFlashRequest = new PieHandlerService.Core.Models.PreFlashOrderRequest
        {
            CertificateChain = ["asdasd"],
            MixNumber = "12323123",
            PreFlash = preFlash
        };
        var mockHandler = new MockHttpMessageHandler();
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create<Services.Pie.Contracts.PreFlashOrderResponse>(preFlashResponse)
        };
        var mockedRequest = mockHandler.When(HttpMethod.Post, Constants.Uri.Pie.FetchPreFlashSoftwareOrder).Respond(mockResponse.StatusCode, mockResponse.Content);
        var httpClient = mockHandler.ToHttpClient();
        var response = _pieSoftwareMetadataService.FetchEcuPreFlashOrder(httpClient, PreFlashRequest);
    }

    [Fact]
    public void Test_FetchPieVehicleCodesOrder()
    {
        var _vehicleObjectBroadcastFile = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.broadcastFile_vehicleobject.json");
        var _pieVehicleCodesResponse = EmbeddedResource.Raw.GetEmbeddedResource(Assembly.GetExecutingAssembly(), "Data.pie_vehicleobjectresponse.json");
        var _vehicleCodesResponse = JsonConvert.DeserializeObject<Services.Pie.Contracts.VehicleCodesResponse>(_pieVehicleCodesResponse);
        var vehicleObjectContext = JsonConvert.DeserializeObject<VehicleObjectContext>(_vehicleObjectBroadcastFile);

        var vehicleObjectRequest = new PieHandlerService.Core.Models.VehicleCodesRequest
        {
            CertificateChain = ["asdasd"],
            MixNumber = "12323123",
            VehicleObject = vehicleObjectContext
        };
        var mockHandler = new MockHttpMessageHandler();
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create<Services.Pie.Contracts.VehicleCodesResponse>(_vehicleCodesResponse)
        };
        var mockedRequest = mockHandler.When(HttpMethod.Post, Constants.Uri.Pie.FetchFactoryVehicleCodes).Respond(mockResponse.StatusCode, mockResponse.Content);
        var httpClient = mockHandler.ToHttpClient();
        var response = _pieSoftwareMetadataService.FetchFactoryVehicleCodes(httpClient, vehicleObjectRequest);
    }
}