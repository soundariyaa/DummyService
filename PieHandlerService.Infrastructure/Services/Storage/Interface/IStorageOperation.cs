using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Storage.Interface;

public interface IStorageOperation
{
    Task<FileStatusDetail> SaveFile(string fileName, string filePath, string content, CancellationToken cancellationToken);

    Task<bool> DeleteFile(string fileName, string filePath, CancellationToken cancellationToken);

    Task<FileStatusDetail> CheckFileExists(string filePath, string fileName, CancellationToken cancellationToken);

    Task<IEnumerable<FileStatusDetail>> SearchFilesByName(string filePath, string regexString, CancellationToken cancellationToken);

    Task<FileStatusDetail> FetchFileContent(string filePath, string fileName, CancellationToken cancellationToken);

    Task<bool> CopyAndExtractFile(string sourceFileLocation, string targetFileLocation, CancellationToken cancellationToken = default);

}