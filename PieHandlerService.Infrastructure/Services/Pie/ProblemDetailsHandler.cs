using System.Dynamic;
using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Extensions;
using PieHandlerService.Infrastructure.Interfaces;
using PieHandlerService.Infrastructure.Services.Pie.Contracts;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using Polly.CircuitBreaker;


namespace PieHandlerService.Infrastructure.Services.Pie;

internal sealed class ProblemDetailsHandler(
    IProblemDetailsManager problemDetailsManager,
    ITimingConfigurationInformer timingConfigurationInformer,
    ILogger<ProblemDetailsHandler> logger)
    :
        ProblemDetailsHandlerBase<ProblemDetailsHandler>(problemDetailsManager, logger),
        IProblemDetailHandler
{
    private readonly ITimingConfigurationInformer _timingConfigurationInformer = timingConfigurationInformer ??
                                                                                 throw new ArgumentNullException(nameof(timingConfigurationInformer));
    private static int Provider => ProblemDetails.Codes.Error200ProviderPie;

    public ProblemDetails Handle(Tuple<Contracts.EndOfLineOrderResponse, StatusCode> input)
    {
        var (endOfLineOrderResponse, statusCode) = input;
        if (!endOfLineOrderResponse.HasError())
        {
            return new ProblemDetails();
        }
        throw new PieSoftwareException(GetProblemDetails(endOfLineOrderResponse, statusCode));
    }

    public ProblemDetails Handle(Tuple<Contracts.PreFlashOrderResponse, StatusCode> input)
    {
        var (preFlashOrderResponse, statusCode) = input;
        if (!preFlashOrderResponse.HasError())
        {
            return new ProblemDetails();
        }
        throw new PieSoftwareException(GetProblemDetails(preFlashOrderResponse, statusCode));
    }

    public ProblemDetails Handle(Tuple<BrokenCircuitException, HttpRequestMessage> input)
    {
        var (brokenCircuitException, httpRequestMessage) = input;

        var problemDetails = ProblemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error20BrokenCircuit];
        problemDetails.MoreInfo.Add(nameof(BrokenCircuitException), new
        {
            Target,
            httpRequestMessage?.Method?.Method,
            Instance = httpRequestMessage?.RequestUri?.ToString(),
            brokenCircuitException?.Message,
            _timingConfigurationInformer?.CircuitBreakerTimingConfiguration?.OpenBrokerTimeoutInSeconds,
            _timingConfigurationInformer?.CircuitBreakerTimingConfiguration?.RetriesBeforeOpening
        });
        problemDetails.Provider = Provider;
        return problemDetails;
    }

    public ProblemDetails Handle(Tuple<HttpRequestException, ExpandoObject> input)
    {
        var (exception, expandoObject) = input;
        dynamic items = expandoObject;

        var request = items?.HttpRequestMessage as HttpRequestMessage;
        var response = items?.HttpResponseMessage as HttpResponseMessage;
        var responseBase = items?.ResponseBase as ResponseBase;

        ProblemDetails problemDetails;
        if (responseBase != null &&
            !responseBase.IsEmpty() &&
            TargetReturnCodeCorrelation.ContainsKey(responseBase.Code) &&
            ProblemDetailsManager.KnownProblemDetails.ContainsKey(TargetReturnCodeCorrelation[responseBase.Code]))
        {
            problemDetails = ProblemDetailsManager.KnownProblemDetails[TargetReturnCodeCorrelation[responseBase.Code]];
        }
        else if (response != null && response.StatusCode.TryMapToCommonProblemDetails(ProblemDetailsManager, out var result))
        {
            problemDetails = result;
        }
        else
        {
            problemDetails =
                ProblemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error1300CommunicationFailure];
        }

        dynamic moreInfoData = new ExpandoObject();
        moreInfoData.Target = Target;
        if (request?.Method?.Method != null) { moreInfoData.Method = request.Method.Method; }
        if (request?.RequestUri != null) { moreInfoData.Instance = request.RequestUri.ToString(); }
        if (response?.StatusCode != null) { moreInfoData.Status = response.StatusCode; }
        if (responseBase?.Code != null) { moreInfoData.Code = responseBase.Code; }
        if (!string.IsNullOrWhiteSpace(responseBase?.Title)) { moreInfoData.Title = responseBase.Title; }
        if (!string.IsNullOrWhiteSpace(responseBase?.ErrorDetail())) { moreInfoData.Detail = responseBase?.ErrorDetail(); }
        if (string.IsNullOrWhiteSpace(responseBase?.ErrorDetail()) &&
            !string.IsNullOrWhiteSpace(response?.ReasonPhrase)) { moreInfoData.Detail = response.ReasonPhrase; }

        problemDetails.MoreInfo.Add(nameof(HttpRequestException), moreInfoData);
        problemDetails.Provider = Provider;
        return problemDetails;
    }

    public ProblemDetails Handle(Tuple<Exception, ExpandoObject> input)
    {
        var (exception, expandoObject) = input;
        dynamic items = expandoObject;

        var httpRequestMessage = items?.HttpRequestMessage as HttpRequestMessage;
        var problemDetails = ProblemDetailsManager.KnownProblemDetails[ProblemDetails.Codes.Error10GeneralExternalSystemError];

        if (items?.HttpResponseMessage is HttpResponseMessage httpResponseMessage && !httpResponseMessage.IsEmpty())
        {
            if (httpResponseMessage.StatusCode.TryMapToCommonProblemDetails(ProblemDetailsManager, out var result))
            {
                problemDetails = result;
            }

            problemDetails.MoreInfo.Add(nameof(Exception), new
            {
                Target,
                httpRequestMessage?.Method?.Method,
                Instance = httpRequestMessage?.RequestUri?.ToString(),
                Status = (int)httpResponseMessage.StatusCode,
                httpResponseMessage.ReasonPhrase
            });

            return problemDetails;
        }
        else
        {
            problemDetails.MoreInfo.Add(nameof(Exception), new
            {
                Target,
                httpRequestMessage?.Method?.Method,
                Instance = httpRequestMessage?.RequestUri?.ToString()
            });
            problemDetails.Detail = exception?.ToString();
        }
        problemDetails.Provider = Provider;
        return problemDetails;
    }


    public ProblemDetails Handle(Tuple<Contracts.VehicleCodesResponse, StatusCode> input)
    {
        var (vehicleCodesResponse, statusCode) = input;
        if (!vehicleCodesResponse.HasError())
        {
            return new ProblemDetails();
        }
        throw new PieSoftwareException(GetProblemDetails(vehicleCodesResponse, statusCode));
    }
}
