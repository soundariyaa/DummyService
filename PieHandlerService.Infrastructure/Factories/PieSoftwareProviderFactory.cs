using AutoMapper;
using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Infrastructure.Services.Pie;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Interface;


namespace PieHandlerService.Infrastructure.Factories;

internal class PieSoftwareProviderFactory(
    IMapper mapper,
    ILoggerFactory loggerFactory,
    IMetricsService metricsService,
    IVbfStorageHandler diskFileHandler,
    IStorageOperation nasStorageOperation,
    IProblemDetailHandler problemDetailsHandler)
    : ISoftwareProviderFactory
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
    private readonly IProblemDetailHandler _problemDetailsHandler = problemDetailsHandler ?? throw new ArgumentNullException(nameof(problemDetailsHandler));
    private readonly IVbfStorageHandler _diskFileHandler = diskFileHandler ?? throw new ArgumentNullException(nameof(_diskFileHandler));
    private readonly IStorageOperation _nasStorageOperation = nasStorageOperation ?? throw new ArgumentNullException(nameof(_nasStorageOperation));

    public ISoftwareMetadataService CreateSoftwareMetadataService()
    {
        return new PieSoftwareMetadataService( _mapper, _loggerFactory.CreateLogger<PieSoftwareMetadataService>(), _metricsService, _diskFileHandler, _nasStorageOperation,_problemDetailsHandler);
    }
}