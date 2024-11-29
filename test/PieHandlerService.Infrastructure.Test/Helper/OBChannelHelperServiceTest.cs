using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Helper;
using PieHandlerService.Infrastructure.Services.Messaging;
using System.Threading.Channels;
using Moq;
using Microsoft.Extensions.Options;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PieHandlerService.Infrastructure.Test.Helper;

public class OBChannelHelperServiceTest
{
    private readonly Mock<ILogger<OBChannelHandlerService>> _logger;
    private readonly Mock<IOptions<ChannelConfig>> _channelConfig;

    private readonly Mock<IHostApplicationLifetime> _lifetime;

    private readonly Mock<IMapper> _mapper;

    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory;

    private readonly Mock<IOptions<MqConfiguration>> _ibmMqConfigurationOptions;

    private readonly Mock<Channel<PieResponseMessage>> _channel;
    private readonly Mock<Channel<PieResponseMessage>> _priorityChannel;

    private readonly OBChannelHandlerService _obChannelHandlerService;

    public OBChannelHelperServiceTest()
    {
        _logger = new Mock<ILogger<OBChannelHandlerService>>();
        _channelConfig = new Mock<IOptions<ChannelConfig>>();
        _lifetime = new Mock<IHostApplicationLifetime>();
        _mapper = new Mock<IMapper>();
        _serviceScopeFactory = new Mock<IServiceScopeFactory>();
        _ibmMqConfigurationOptions = new Mock<IOptions<MqConfiguration>>();
        _channel = new Mock<Channel<PieResponseMessage>>();
        _priorityChannel = new Mock<Channel<PieResponseMessage>>();
        _obChannelHandlerService = new OBChannelHandlerService(_lifetime.Object, _mapper.Object, _serviceScopeFactory.Object,  _logger.Object, _ibmMqConfigurationOptions.Object,_channelConfig.Object);
    }

    [Fact(Skip = "MQ test setup required")]
    public void Test_HandlePieOrderMessage() {
        var pieOrderMessage = new PieResponseMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = SIIGOrderType.SIIGOrderVehicleKeys.ToString(),
            Status = OrderStatus.Available,
            ContentHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            FileName = "sample.json",
            IsPriority = false
        };
        var responseTask = _obChannelHandlerService.HandleMessage(pieOrderMessage, CancellationToken.None);
        Assert.True(responseTask.IsCompleted);
    }
}