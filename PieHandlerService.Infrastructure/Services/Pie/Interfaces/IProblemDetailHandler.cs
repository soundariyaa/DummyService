using PieHandlerService.Core.Interfaces;
using PieHandlerService.Infrastructure.Services.Pie.Contracts;
using Polly.CircuitBreaker;
using System.Dynamic;


namespace PieHandlerService.Infrastructure.Services.Pie.Interfaces;

public interface IProblemDetailHandler : IProblemDetailsHandler<Tuple<EndOfLineOrderResponse, StatusCode>>,
    IProblemDetailsHandler<Tuple<VehicleCodesResponse, StatusCode>>,
    IProblemDetailsHandler<Tuple<PreFlashOrderResponse, StatusCode>>,
    IProblemDetailsHandler<Tuple<BrokenCircuitException, HttpRequestMessage>>,
    IProblemDetailsHandler<Tuple<HttpRequestException, ExpandoObject>>,
    IProblemDetailsHandler<Tuple<Exception, ExpandoObject>>
{
}