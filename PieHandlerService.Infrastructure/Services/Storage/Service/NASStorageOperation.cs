using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Exceptions;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Interface;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;
using System.IO.Compression;
using PieHandlerService.Infrastructure.Extensions;

namespace PieHandlerService.Infrastructure.Services.Storage.Service;

internal class NasStorageOperation(ILogger<NasStorageOperation> logger, IProblemDetailHandler problemDetailsHandler) : IStorageOperation
{
    private readonly ILogger<NasStorageOperation> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProblemDetailHandler _problemDetailsHandler = problemDetailsHandler ?? throw new ArgumentNullException(nameof(problemDetailsHandler));

    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public Task<FileStatusDetail> CheckFileExists(string filePath, string fileName, CancellationToken cancellationToken = default)
    {
        var isExists = File.Exists(filePath + Path.DirectorySeparatorChar + fileName);
        _logger.LogInformation("File with name {FileName} in the path {StorageLocation} exists", fileName, filePath);
        return Task.FromResult<FileStatusDetail>(new FileStatusDetail(FileStatusType.UPDATED, fileName, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), isExists));
    }

    public async Task<bool> DeleteFile(string fileName, string filePath, CancellationToken cancellationToken = default)
    {
        bool IsDeleted = false;
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            if (File.Exists(filePath + Path.DirectorySeparatorChar + fileName))
            {
                File.Delete(filePath + Path.DirectorySeparatorChar + fileName);
                IsDeleted = true;
                _logger.LogInformation("Deleted file with name {FileName} in the path {StorageLocation} successfully", fileName, filePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting file with name {FileName} in the {StorageLocation}", fileName, filePath);
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FileName = fileName;
            throw new StorageException(_problemDetailsHandler.Handle(expandoObject));
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        return IsDeleted;
    }

    public async Task<FileStatusDetail> FetchFileContent(string folderPath, string fileName, CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            var filePath = Path.Combine(folderPath, fileName);
            var isExist = false;
            var fileContent = string.Empty;
            if (File.Exists(filePath))
            {
                fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
                isExist = true;
            }
            var fileStatus = new FileStatusDetail(FileStatusType.UPDATED, fileName, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), isExist)
            {
                Content = fileContent
            };
             _logger.LogInformation("Fetched content for the file with name {FileName} in the path {StorageLocation}", fileName, folderPath);
            return fileStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching content of file with name {FileName} in the {StorageLocation}", fileName, folderPath);
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FileName = fileName;
            throw new StorageException(_problemDetailsHandler.Handle(expandoObject));
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<bool> CopyAndExtractFile(string sourceFileLocation, string mixNumber, CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        var fileName = (!string.IsNullOrEmpty(sourceFileLocation) ? sourceFileLocation.Split(Path.DirectorySeparatorChar)
            [sourceFileLocation.Split(Path.DirectorySeparatorChar).Length - 1] : string.Empty);
        var fileLocation = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASPIEVinUniqueLocation) + Path.DirectorySeparatorChar + fileName;
        var targetFileLocation = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.NASOEVinUniqueLocationPreFix) + Path.DirectorySeparatorChar + Constants.Factory.EOLZone +
                                 Path.DirectorySeparatorChar + mixNumber + Path.DirectorySeparatorChar + fileName.Substring(0, fileName.Length - 4);
        try
        {
            _logger.LogInformation("Extracting {StorageLocation} to {TargetFileLocation} (MixNumber: {MixNumber})", fileLocation, targetFileLocation, mixNumber);
            if (File.Exists(fileLocation))
            {
                Directory.CreateDirectory(targetFileLocation);
                ZipFile.ExtractToDirectory(fileLocation,
                    targetFileLocation);
                return true;
            }
            else
            {
                _logger.LogError("Error finding the VIN unique zip from the location {StorageLocation}", fileLocation);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogException(ex);
            _logger.LogError("Error occurred while trying to copy VIN unique file from {StorageLocation} to {TargetFileLocation}", fileLocation, targetFileLocation);
            return false;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<FileStatusDetail> SaveFile(string fileName, string filePath, string content, CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            await File.WriteAllTextAsync(filePath + Path.DirectorySeparatorChar + fileName + Constants.FileExtentions.JSON, content, cancellationToken);
            var fileStatus = new FileStatusDetail(FileStatusType.CREATED, fileName + Constants.FileExtentions.JSON, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), true)
            {
                FileHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(content)))
            };
            _logger.LogInformation("Saved file with name {FileName} in the path {StorageLocation} successfully", fileName, filePath);
            return fileStatus;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving file with name {FileName} in the {StorageLocation}", fileName, filePath);
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FileName = fileName;
            throw new StorageException(_problemDetailsHandler.Handle(expandoObject));
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<IEnumerable<FileStatusDetail>> SearchFilesByName(string filePath, string regexString, CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            var categoryFiles = Directory.GetFiles(filePath).AsEnumerable();
            var fileStatusDetails = new List<FileStatusDetail>();
            if (categoryFiles.Count() > 0)
            {
                categoryFiles.AsParallel().ForAll(x =>
                {
                    if (x.StartsWith(regexString))
                    {
                        var lastUpdatedTs = Directory.GetLastWriteTimeUtc(filePath + Path.DirectorySeparatorChar + x);
                        fileStatusDetails.Add(new FileStatusDetail(FileStatusType.UPDATED, x, new DateTimeOffset(lastUpdatedTs).ToUnixTimeMilliseconds(), true));
                    }
                });
            }
            _logger.LogInformation("Found {Count} number of files based on search criteria", fileStatusDetails.Count);
            return fileStatusDetails;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching files with string {RegexString} in the {FilePath}", regexString, filePath);
            dynamic expandoObject = new ExpandoObject();
            expandoObject.FilePath = filePath;
            expandoObject.RegexString = regexString;
            throw new StorageException(_problemDetailsHandler.Handle(expandoObject));
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}