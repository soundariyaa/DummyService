using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PieHandlerService.Application.Services;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using System.Linq.Expressions;

namespace PieHandlerService.Application.Test;

public class OrderDetailServiceTest
{
    private readonly Mock<ISiigOrderRepository> _siigOrderRepository;
    private readonly Mock<ILogger<OrderDetailService>> _loggerMock;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<IProblemDetailsManager> _problemDetailsManager;

    private readonly IOrderDetailService _orderDetailService;

    public OrderDetailServiceTest() {
        _siigOrderRepository = new Mock<ISiigOrderRepository>();
        _loggerMock = new Mock<ILogger<OrderDetailService>>();
        _mapper = new Mock<IMapper>();
        _problemDetailsManager = new Mock<IProblemDetailsManager>();
        _orderDetailService = new OrderDetailService(_siigOrderRepository.Object, _problemDetailsManager.Object,  _loggerMock.Object);
    }

    [Fact]
    public void Test_FetchOrderDetails()
    {
        var _createOrderSpecification = new SiigOrderQuerySpecification("12312321", "318b316c-405e-4df0-b272-93d155fdc120", SIIGOrderType.SIIGOrderEndOfLine);


        _siigOrderRepository.Setup(x => x.FetchAll(It.IsAny<Expression<Func<SiigOrderQuerySpecification, bool>>>())).Returns(
            Task.FromResult((IEnumerable<SiigOrder>)new List<SiigOrder>
            {
                new SiigOrder
                {
                    OeIdentifier = Guid.NewGuid().ToString(),
                    MixNumber = "12345678",
                    CreatedUtcTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    ModifiedUtcTs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    OrderType = SIIGOrderType.SIIGOrderEndOfLine,
                    OrderStatus = OrderStatus.Available,
                    OrderResponse = "\"{\\\"createdUtcMs\\\":1712673207949,\\\"forcedOverrideCompletionRule\\\":true,\\\"isPriority\\\":false,\\\"mixNumber\\\":\\\"5036299\\\",\\\"oeId\\\":\\\"318b316d-405e-4df0-b272-93d155fdc120\\\",\\\"originHash\\\":\\\"d95e6a81b4a4009f34182a870427d9d55faa282eecf908e6d4f0670743307ee1\\\",\\\"preFlashContext\\\":{\\\"createdUtcMs\\\":1712673207949,\\\"ecus\\\":[{\\\"authenticationTemplate\\\":\\\"32218512\\\",\\\"ecuName\\\":\\\"HIA\\\",\\\"software\\\":[{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413428\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413452\\\",\\\"state\\\":\\\"Unknown\\\"}]},{\\\"authenticationTemplate\\\":\\\"32218512\\\",\\\"ecuName\\\":\\\"HIB\\\",\\\"software\\\":[{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32437017\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32437024\\\",\\\"state\\\":\\\"Unknown\\\"}]},{\\\"authenticationTemplate\\\":\\\"32375204\\\",\\\"ecuName\\\":\\\"HPA\\\",\\\"software\\\":[{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413209\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413225\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413229\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413396\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413398\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413400\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413402\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413404\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413406\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413408\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413967\\\",\\\"state\\\":\\\"Unknown\\\"}]},{\\\"authenticationTemplate\\\":\\\"32218512\\\",\\\"ecuName\\\":\\\"LPA\\\",\\\"software\\\":[{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413416\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413418\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413420\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413422\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413424\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413426\\\",\\\"state\\\":\\\"Unknown\\\"}]},{\\\"authenticationTemplate\\\":\\\"32218512\\\",\\\"ecuName\\\":\\\"SGA\\\",\\\"software\\\":[{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413410\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413412\\\",\\\"state\\\":\\\"Unknown\\\"},{\\\"modifiedUtcMs\\\":1712673400958,\\\"partNumber\\\":\\\"32413414\\\",\\\"state\\\":\\\"Unknown\\\"}]}],\\\"modifiedUtcMs\\\":1712673207949,\\\"preFlashHash\\\":\\\"a9be90728f43c2897c6c086c90a77dd90be8d23d3f279cbce636780169a7c66b\\\",\\\"state\\\":\\\"Pending\\\"}}\"\r\n"
                }
            }));

        var response = _orderDetailService.FetchOrderDetails(_createOrderSpecification);
        Assert.NotNull(response);
        Assert.True(response.Result.Count() > 0);

    }

}