using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Helper;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using Moq;
using Microsoft.Extensions.Options;
using PieHandlerService.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using PieHandlerService.Infrastructure.Services.Messaging;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
namespace PieHandlerService.Infrastructure.Test.Helper;

public class IBChannelHelperServiceTest
{

    private readonly Mock<ILogger<IBChannelHandlerService>> _logger ;

    private readonly Mock<IStorageHandlerService> _storageHandlerService;

    private readonly Mock<IHostApplicationLifetime> _lifetime;

    private readonly Mock<IMapper> _mapper;

    private readonly Mock<IServiceScopeFactory> _serviceScopeFactory;

    private readonly Mock<IOptions<MqConfiguration>> _ibmMqConfigurationOptions;

    private readonly Mock<IProblemDetailsManager> _problemDetailsHandler;

    private readonly Mock<IOptions<ChannelConfig>> _channelConfig;
    private readonly Mock<IBroadcastMetadataRepository> _broadcastMetadataRepo;

    private readonly IBChannelHandlerService _ibChannelHandlerService;

    public IBChannelHelperServiceTest() {
        _logger = new Mock<ILogger<IBChannelHandlerService>>();
        _lifetime = new Mock<IHostApplicationLifetime>();
        _mapper = new Mock<IMapper>();
        _serviceScopeFactory = new Mock<IServiceScopeFactory>();
        _ibmMqConfigurationOptions = new Mock<IOptions<MqConfiguration>>();
        _channelConfig = new Mock<IOptions<ChannelConfig>>();
        _ibChannelHandlerService = new IBChannelHandlerService(_lifetime.Object, _mapper.Object, _ibmMqConfigurationOptions.Object, _channelConfig.Object, _logger.Object, _serviceScopeFactory.Object);
    }

    [Fact(Skip = "MQ test setup required")]
    public void Test_ProcessBroadcastMessage() {
        var broadcastMessage = new BroadcastContextMessage
        {
            OeId = Guid.NewGuid().ToString(),
            MixNumber = "14141234",
            CreatedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ModifiedUtcMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            RequestType = RequestType.VehicleObject,
            Provider = "BroadcastHandlerService",
            Independent = false,
            ContentHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
            FileName = "sample.json",
            IsPriority = false
        };
        _broadcastMetadataRepo.Setup(x => x.Add(It.IsAny<BroadcastContextMessage>(),CancellationToken.None)).Returns(Task.CompletedTask);
        var response = _ibChannelHandlerService.PublishMessageToChannel(broadcastMessage,CancellationToken.None);
        Assert.NotNull(response);
    }

    [Fact(Skip = "MQ test setup required")]
    public void Test_PublishMessageToChannel()
    {
        var response = _ibChannelHandlerService.ProcessBroadcastMessage(CancellationToken.None,false);
        Assert.NotNull(response);
    }
}