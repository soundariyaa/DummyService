using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Storage.Interface;

namespace PieHandlerService.Infrastructure.Services.Storage.Service;

internal class VbfStorageHandler(ILogger<VbfStorageHandler> logger, IProblemDetailsManager problemDetailsManager, IMetricsService metricsService) : IVbfStorageHandler
{
    private readonly ILogger<VbfStorageHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProblemDetailsManager _problemDetailsManager = problemDetailsManager ?? throw new ArgumentNullException(nameof(problemDetailsManager));
    private readonly IMetricsService _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));

    public IEnumerable<FileStatusDetail> FetchVbfFileStatus(string mixNumber, VbfCategory vbfCategory, IEnumerable<FileMetaData> fileMetadata, SIIGOrderType orderType)
    {
        _logger.LogInformation($"Started with method {nameof(VbfStorageHandler)} {nameof(FetchVbfFileStatus)}");

        var categoryFiles = FetchVbfFileNames(mixNumber, vbfCategory);

        var subDirectory = (SIIGOrderType.SIIGOrderEndOfLine.Equals(orderType) ? Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASEndOfLineSubDirectory) :
            (SIIGOrderType.SIIGOrderPreFlash.Equals(orderType) ? Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPreFlashSubDirectory) : throw new ArgumentException(nameof(orderType))));

        var fileStatusDetails = new List<FileStatusDetail>();
        fileMetadata.AsParallel().ForAll(x =>
        {
            var fileStatus = new FileStatusDetail(FileStatusType.UPDATED, x.FileName, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), categoryFiles.Contains(x.FileName));
            fileStatusDetails.Add(fileStatus);
        });
        _logger.LogInformation("Fetched VBF files count of {Count} for the MixNumber {MixNumber}", fileStatusDetails.Count, mixNumber);
        return fileStatusDetails;
    }

    private List<string> FetchVbfFileNames(string mixNumber, VbfCategory vbfCategory)
    {
        var pieFolderPath = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPIEVbfLocation)!;
        var categoryFiles = new List<string>();
        if (vbfCategory == VbfCategory.ALL)
        {
            var carconfig = CheckAndFetchFileNames(pieFolderPath, mixNumber, VbfCategory.CARCONFIG.ToString());
            categoryFiles.AddRange(carconfig);

            var staticFiles = CheckAndFetchFileNames(pieFolderPath, mixNumber, VbfCategory.STATIC.ToString());
            categoryFiles.AddRange(staticFiles);

            var vinUnique = CheckAndFetchFileNames(pieFolderPath, mixNumber, VbfCategory.VIN_UNIQUE.ToString());
            categoryFiles.AddRange(vinUnique);
        }
        else
        {
            var files = CheckAndFetchFileNames(pieFolderPath, mixNumber, vbfCategory.ToString());
            categoryFiles.AddRange(files);
        }
        return categoryFiles;
    }

    private static IEnumerable<string> CheckAndFetchFileNames(string pieFolderPath, string mixNumber, string vbfCategory)
    {
        var directoryPath = Path.Combine(pieFolderPath, mixNumber, vbfCategory);
        return Directory.Exists(directoryPath) ? Directory.GetFiles(directoryPath) : [];
    }

    /*
     * Does Pie Response match VBF location files
     */
    public async Task<bool> IsPieResponseMatchVbfLocationFiles(string mixNumber, List<FileMetaData> fileNames, SIIGOrderType orderType) {

        var isSuccess = false;
        var vbfValidationRequired =
            (Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.PieVbfValidation) ?? "false")
            .Equals(true.ToString(), StringComparison.OrdinalIgnoreCase);

        if (!vbfValidationRequired) return isSuccess;

        var fileStatuses =  FetchVbfFileStatus(mixNumber, VbfCategory.VIN_UNIQUE, fileNames, orderType);
        var integrityCheck = await PerformIntegrityCheck(fileNames, fileStatuses);
        _metricsService.IncreasePieRequestCounter(
            Constants.Metrics.HandledPieMissingVbfs,
            (!integrityCheck) ? Constants.Metrics.FailureValue : Constants.Metrics.SuccessValue);
        if (!integrityCheck)
        {
            _logger.LogError($"Mismatch in the VBF files between PIE response and NAS storage");
        }
        isSuccess = integrityCheck;
        return isSuccess;
    }


    /**
    * Performs the PIE VBf file integrity check
    * where VBFs listed by PIE and the ones available in NAS location do match
    */
    private Task<bool> PerformIntegrityCheck(IEnumerable<FileMetaData> fileMetadataList, IEnumerable<FileStatusDetail> fileStatusDetails)
    {
        var IsMatching = true;

        fileMetadataList.AsParallel().ForAll(fileMetadata =>
        {
            var result = fileStatusDetails.Where(x => x.FileName.Equals(fileMetadata.FileName) && x.IsAvailable).ToList();
            IsMatching = (IsMatching && result.Any());
        });

        return Task.FromResult<bool>(IsMatching);
    }
}