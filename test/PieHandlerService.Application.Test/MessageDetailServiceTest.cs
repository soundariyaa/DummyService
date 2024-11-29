using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PieHandlerService.Application.Services;
using PieHandlerService.Core.Interface;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Linq.Expressions;

namespace PieHandlerService.Application.Test;

public class MessageDetailServiceTest
{
    private readonly IMessageDetailService _messageDetailService;

    private readonly Mock<IBroadcastMetadataRepository> _broadcastMetadataRepo;
    private readonly Mock<IPieOrderRepository> _pieOrderRepo;
    private readonly Mock<ILogger<MessageDetailService>> _loggerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IProblemDetailsManager> _problemDetailsManager;

    public MessageDetailServiceTest() {
        _broadcastMetadataRepo = new Mock<IBroadcastMetadataRepository>();
        _pieOrderRepo = new Mock<IPieOrderRepository>();
        _loggerMock = new Mock<ILogger<MessageDetailService>>();
        _mapperMock = new Mock<IMapper>();
        _problemDetailsManager = new Mock<IProblemDetailsManager>();
        _messageDetailService = new MessageDetailService(_broadcastMetadataRepo.Object, _pieOrderRepo.Object, _problemDetailsManager.Object,  _loggerMock.Object);
    }


    [Fact]
    public async Task Test_FetchBroadcastMessageDetails() 
    {
        var createOrderSpecification = new BroadcastMessageSpecification("12312321", "318b316c-405e-4df0-b272-93d155fdc120", RequestType.EndOfLine);

        _broadcastMetadataRepo.Setup(x => x.FetchAll(It.IsAny<Expression<Func<BroadcastMessageSpecification, bool>>>())).Returns(
            Task.FromResult((IEnumerable<BroadcastContextMessage>)new List<BroadcastContextMessage>
            {
                new BroadcastContextMessage
                {
                    OeId = Guid.NewGuid().ToString(),
                    MixNumber = "12345678",
                    CreatedUtcMs = 1712673207949,
                    ModifiedUtcMs = 1712673207949,
                    RequestType = RequestType.EndOfLine,
                    Status = OrderStatus.Available,
                    Provider = "BroadcastHandlerService",
                    Independent = false,
                    FileName = "5036279_318b316c_1714032168088_endOfLine.json",
                    ContentHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
                    OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
                    IsPriority = false,

                }
            }));

        var response = await _messageDetailService.FetchBroadcastMessageDetails(createOrderSpecification);
        Assert.NotNull(response);
        Assert.True(response.Any());
    }


    [Fact]
    public async Task Test_FetchPieMessageDetails()
    {
        var _createOrderSpecification = new PieMessageSpecification("12312321", "318b316c-405e-4df0-b272-93d155fdc120", SIIGOrderType.SIIGOrderEndOfLine);

        _pieOrderRepo.Setup(x => x.FetchAll(It.IsAny<Expression<Func<PieMessageSpecification, bool>>>())).Returns(
            Task.FromResult((IEnumerable<PieResponseMessage>)new List<PieResponseMessage>
            {
                new PieResponseMessage
                {
                    OeId = Guid.NewGuid().ToString(),
                    MixNumber = "12345678",
                    CreatedUtcMs = 1712673207949,
                    ModifiedUtcMs = 1712673207949,
                    RequestType = SIIGOrderType.SIIGOrderEndOfLine.ToString(),
                    Status = OrderStatus.Available,
                    FileName = "5036279_318b316c_1714032168088_endOfLine.json",
                    ContentHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
                    OriginHash = "d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1",
                    IsPriority = false
                }
            }));

        var response = await _messageDetailService.FetchPieMessageDetails(_createOrderSpecification);
        Assert.NotNull(response);
        Assert.True(response.Any());
    }
}