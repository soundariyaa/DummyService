using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Extensions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Extensions;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using Polly.CircuitBreaker;
using System.Dynamic;
using System.Text;

namespace PieHandlerService.Infrastructure.Services.Pie;

public class PieSoftwareMetadataService(
    IMapper mapper,
    ILogger<PieSoftwareMetadataService> logger,
    IMetricsService metricsService,
    IVbfStorageHandler vbfStorageHandler,
    IStorageOperation nasStorageOperation,
    IProblemDetailHandler problemDetailsHandler)
    : ISoftwareMetadataService
{
    private readonly ILogger<PieSoftwareMetadataService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
    private readonly IVbfStorageHandler _vbfStorageHandler = vbfStorageHandler ?? throw new ArgumentNullException(nameof(vbfStorageHandler));
    private readonly IStorageOperation _nasStorageOperation = nasStorageOperation ?? throw new ArgumentNullException(nameof(nasStorageOperation));
    private readonly IProblemDetailHandler _problemDetailsHandler = problemDetailsHandler ?? throw new ArgumentNullException(nameof(problemDetailsHandler));

    private const string ClassName = nameof(PieSoftwareMetadataService);

    private async Task StorePieTrace(string jsonSerializedObject, string mixNumber, string requestType, bool isRequest, bool isSuccess)
    {
        var nasPieStorageLocation = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPIEStorageLocation)
                                    ?? throw new NullReferenceException($"The environment variable {Constants.EnvironmentVariables.NASPIEStorageLocation} is not set");

        var mixNumberFolder = Path.Combine(nasPieStorageLocation, mixNumber);
        var fileName = isRequest
            ? mixNumber + "_" + requestType + "_PieRequest"
            : isSuccess
                ? mixNumber + "_" + requestType + "_PieResponse_PieSuccess"
                : mixNumber + "_" + requestType + "_PieResponse_PieFailure";
        
        fileName += "_" + DateTimeOffset.Now.ToUnixTimeSeconds();

        await _nasStorageOperation.SaveFile(fileName, mixNumberFolder, jsonSerializedObject, default);
    }

    private static bool IsPieTraceEnabled () 
    {
        _ = bool.TryParse(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.PieTraceEnabled), out bool pieTraceEnabled);
        return pieTraceEnabled;
    }

    public async Task<PreFlashOrderResponse> FetchEcuPreFlashOrder(HttpClient httpClient, PreFlashOrderRequest preFlashOrderRequest)
    {
        _logger.LogInformation("{ClassName}.{MethodName} start for MixNumber: {MixNumber}", ClassName, nameof(FetchEcuPreFlashOrder), preFlashOrderRequest?.MixNumber);

        var request = new HttpRequestMessage();
        var response = new HttpResponseMessage();
        var successfullyHandled = false;
        var getPreFlashOrderRequest = _mapper.Map<Contracts.PreFlashOrderRequest>(preFlashOrderRequest);
        var getPreFlashOrderResponse = new Contracts.PreFlashOrderResponse();
        var jsonSerializedContent = JsonConvert.SerializeObject(getPreFlashOrderRequest);

        if (IsPieTraceEnabled()) { await StorePieTrace(jsonSerializedContent, preFlashOrderRequest.MixNumber, RequestType.PreFlash.ToString(),true,false); } 
            
        request = new HttpRequestMessage().GetPreFlashSoftwareOrder(
            new StringContent(jsonSerializedContent, Encoding.UTF8, "application/json"),
            httpClient,
            Constants.Uri.Pie.FetchPreFlashSoftwareOrder);
        request.LogRequest(nameof(FetchEcuPreFlashOrder), _logger);
        try
        {
            response = await ProcessHttpRequest(request, httpClient, preFlashOrderRequest?.MixNumber, RequestType.PreFlash.ToString());
            getPreFlashOrderResponse = await response.ContentAsType<Contracts.PreFlashOrderResponse>();

            if (IsPieTraceEnabled()) {  await StorePieTrace(JsonConvert.SerializeObject(getPreFlashOrderResponse), preFlashOrderRequest.MixNumber, RequestType.PreFlash.ToString(), false, true); }

            _logger.LogInformation("PIE PreFlash API call returned successfully for Mix Number {MixNumber}", getPreFlashOrderRequest?.MixNumber);
            var fileNames = new List<FileMetaData>();
            getPreFlashOrderResponse?.Ecus?.AsParallel().ForAll(x =>
            {
                x?.Software?.AsParallel().ForAll(y =>
                {
                    fileNames.Add(new FileMetaData(y.FileName, 0));
                });
            });
            await _vbfStorageHandler.IsPieResponseMatchVbfLocationFiles(preFlashOrderRequest.MixNumber, fileNames, SIIGOrderType.SIIGOrderPreFlash);
            successfullyHandled = true;
        }
        finally
        {
            _metricsService.IncreasePieRequestCounter(
                Constants.Metrics.HandledPiePreFlashRequestsMetric,
                successfullyHandled ? Constants.Metrics.SuccessValue : Constants.Metrics.FailureValue);
        }
        var problemDetails = _problemDetailsHandler
            .Handle(new Tuple<Contracts.PreFlashOrderResponse, StatusCode>(
                getPreFlashOrderResponse,
                new StatusCode { Value = (int)response.StatusCode }));

        _logger.LogInformation("{@ProblemDetails}", problemDetails);

        var result = _mapper.Map<PreFlashOrderResponse>((object)getPreFlashOrderResponse) ?? new PreFlashOrderResponse();
        result.ProblemDetails = problemDetails;
        return result;
    }

    public async Task<VehicleCodesResponse> FetchFactoryVehicleCodes(HttpClient httpClient, VehicleCodesRequest vehicleCodesRequest)
    {
        _logger.LogInformation("{ClassName}.{MethodName} start for MixNumber: {MixNumber}", ClassName, nameof(FetchFactoryVehicleCodes), vehicleCodesRequest?.MixNumber);
        var request = new HttpRequestMessage();
        var response = new HttpResponseMessage();
        var getVehicleCodesRequest = _mapper.Map<Contracts.VehicleCodesRequest>(vehicleCodesRequest);
        var successfullyHandled = false;

        var jsonSerializedContent = JsonConvert.SerializeObject(getVehicleCodesRequest);
        if (IsPieTraceEnabled()) { await StorePieTrace(jsonSerializedContent, vehicleCodesRequest.MixNumber, RequestType.VehicleObject.ToString(), true, false); }
        request = new HttpRequestMessage().GetFactoryVehicleCodes(
            new StringContent(jsonSerializedContent, Encoding.UTF8, "application/json"),
            httpClient,
            Constants.Uri.Pie.FetchFactoryVehicleCodes);
        request.LogRequest(nameof(FetchFactoryVehicleCodes), _logger);

        try
        {
            response = await ProcessHttpRequest(request, httpClient, vehicleCodesRequest.MixNumber, RequestType.VehicleObject.ToString());
            successfullyHandled = true;
            var vehicleCodesResponse = await response.ContentAsType<Contracts.VehicleCodesResponse>();

            if (IsPieTraceEnabled()) { await StorePieTrace(JsonConvert.SerializeObject(vehicleCodesResponse), vehicleCodesRequest.MixNumber, RequestType.VehicleObject.ToString(), false, true); }

            _logger.LogInformation("PIE Vehicle Code API call returned successfully for Mix Number {MixNumber}", getVehicleCodesRequest?.MixNumber);

            var problemDetails = _problemDetailsHandler
                .Handle(new Tuple<Contracts.VehicleCodesResponse, StatusCode>(
                    vehicleCodesResponse,
                    new StatusCode { Value = (int)response.StatusCode }));

            _logger.LogInformation("{@ProblemDetails}", problemDetails);
            var result = _mapper.Map<VehicleCodesResponse>(vehicleCodesResponse) ?? new VehicleCodesResponse();
            result.ProblemDetails = problemDetails;
            return result;

        }
        finally
        {
            _metricsService.IncreasePieRequestCounter(
                Constants.Metrics.HandledPieVehicleObjectRequestsMetric,
                successfullyHandled ? Constants.Metrics.SuccessValue : Constants.Metrics.FailureValue);
        }
    }

    public async Task<EndOfLineOrderResponse> FetchEcuEndOfLineOrder(HttpClient httpClient, EndOfLineOrderRequest endOfLineOrderRequest)
    {
        _logger.LogInformation("{ClassName}.{MethodName} start for MixNumber: {MixNumber}", ClassName, nameof(FetchEcuEndOfLineOrder), endOfLineOrderRequest?.MixNumber);

        var successfullyHandled = false;
        var getEndOfLineOrderRequest = _mapper.Map<Contracts.EndOfLineOrderRequest>(endOfLineOrderRequest);
        var getEndOfLineOrderResponse = new Contracts.EndOfLineOrderResponse();
        var jsonSerializedContent = JsonConvert.SerializeObject(getEndOfLineOrderRequest);

        if (IsPieTraceEnabled()) { await StorePieTrace(jsonSerializedContent, endOfLineOrderRequest.MixNumber, RequestType.EndOfLine.ToString(), true, false); }
        var request = new HttpRequestMessage().GetEndOfLineSoftwareOrder(
            new StringContent(jsonSerializedContent, Encoding.UTF8, "application/json"),
            httpClient,
            Constants.Uri.Pie.FetchEndOfLineSoftwareOrder);
        request.LogRequest(nameof(FetchEcuEndOfLineOrder), _logger);
        _ = int.TryParse(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.RetryCount), out int retryCount);
        _ = int.TryParse(Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.RetryDelaySeconds), out int retryDelaySeconds);
        try
        {
            HttpResponseMessage response;
            do
            {
                response = await ProcessHttpRequest(request, httpClient, endOfLineOrderRequest.MixNumber, RequestType.EndOfLine.ToString());
                getEndOfLineOrderResponse = await response.ContentAsType<Contracts.EndOfLineOrderResponse>();
                if (IsPieTraceEnabled()) { await StorePieTrace(JsonConvert.SerializeObject(getEndOfLineOrderResponse), endOfLineOrderRequest.MixNumber, RequestType.EndOfLine.ToString(), false, true); }
                if (getEndOfLineOrderResponse.HasError() && getEndOfLineOrderResponse.HasRetryableError()) {
                    await Task.Delay(retryDelaySeconds*1000);
                    retryCount--;
                }
            }
            while (getEndOfLineOrderResponse.HasError() && getEndOfLineOrderResponse.HasRetryableError() && retryCount > 0);
            _logger.LogInformation("PIE End Of Line (EOL) API call returned successfully for Mix Number {MixNumber}", getEndOfLineOrderRequest?.MixNumber);
            var fileNames = new List<FileMetaData>();
            getEndOfLineOrderResponse?.Order?.LoadEcuSet?.Ecus?.AsParallel().ForAll(x =>
            {
                x?.Software?.AsParallel().ForAll(y =>
                {
                    fileNames.Add(new FileMetaData(y.FileName, 0));
                });
            });

            _logger.LogDebug("Copy and extract {StorageLocation} for MixNumber: {MixNumber}",
                getEndOfLineOrderResponse?.StorageLocation, endOfLineOrderRequest.MixNumber);

            var isCopySuccess = false;
            if (!string.IsNullOrEmpty(getEndOfLineOrderResponse.StorageLocation)) {
               isCopySuccess =  await _nasStorageOperation.CopyAndExtractFile(getEndOfLineOrderResponse.StorageLocation,
                    endOfLineOrderRequest.MixNumber);
            }

            if (!isCopySuccess) {
                var problemDetail = new ProblemDetails(12345, "Error copying and unzipping VIN unique after PIE EOL API Request", 0, "Error copying VIN unique files");
                throw new PieSoftwareException(problemDetail);
            }

            await _vbfStorageHandler.IsPieResponseMatchVbfLocationFiles(endOfLineOrderRequest.MixNumber, fileNames, SIIGOrderType.SIIGOrderEndOfLine);
            successfullyHandled = true;

            var problemDetails = _problemDetailsHandler.Handle(new Tuple<Contracts.EndOfLineOrderResponse, StatusCode>(
                getEndOfLineOrderResponse ?? new Contracts.EndOfLineOrderResponse(), new StatusCode { Value = (int)response.StatusCode }));

            _logger.LogInformation("{@ProblemDetails}", problemDetails);

            var result = _mapper.Map<Core.Models.EndOfLineOrderResponse>(getEndOfLineOrderResponse) ?? new EndOfLineOrderResponse();
            result.ProblemDetails = problemDetails;
            return result;
        }
        finally
        {
            _metricsService.IncreasePieRequestCounter(
                Constants.Metrics.HandledPieEndOfLineRequestsMetric,
                successfullyHandled ? Constants.Metrics.SuccessValue : Constants.Metrics.FailureValue);
        }
    }

    /**
     * process PIE http calls
     */
    private async Task<HttpResponseMessage> ProcessHttpRequest(HttpRequestMessage request, HttpClient httpClient, string mixNumber, string requestType)
    {
        var response = new HttpResponseMessage();
        try
        {
            _logger.LogDebug("PIE request to {RequestUri}", request.RequestUri);
            response = await httpClient.SendAsync(request);
            await response.EnsureSuccessStatusCode(_logger);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogException(ex);
            dynamic expandoObject = new ExpandoObject();
            expandoObject.HttpRequestMessage = request;
            expandoObject.HttpResponseMessage = response;
            var jsonResponse = await response.ContentAsType<Contracts.ResponseBase>();
            var jsonSerializedContent = JsonConvert.SerializeObject(jsonResponse);
            if (IsPieTraceEnabled()) { await StorePieTrace(jsonSerializedContent, mixNumber, requestType, false, false); }
            _logger.LogError(ex, "PIE failed call to {RequestUri} with response : {JsonSerializedContent}", request.RequestUri, jsonSerializedContent);
            var problemDetail = new ProblemDetails(jsonResponse.Code,jsonResponse.Detail,jsonResponse.Status,jsonResponse?.Title);
            throw new PieSoftwareException(problemDetail);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogException(ex);
            var problemDetail = new ProblemDetails(ProblemDetails.Codes.Error20BrokenCircuit, ex.ToString(), 503, "Service Unavailable");
            throw new PieSoftwareException(problemDetail);
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
            var problemDetail = new ProblemDetails(ProblemDetails.Codes.Error10GeneralExternalSystemError, ex.ToString(), 503, "Unhandled General Exception");
            throw new PieSoftwareException(problemDetail);
        }
        return response;
    }

}