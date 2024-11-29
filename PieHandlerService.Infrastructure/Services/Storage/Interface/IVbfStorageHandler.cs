using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Services.Storage.Interface;

public interface IVbfStorageHandler
{

    IEnumerable<FileStatusDetail> FetchVbfFileStatus(string mixNumber, VbfCategory vbfCategory, IEnumerable<FileMetaData> fileMetadata, SIIGOrderType orderType);

    Task<bool> IsPieResponseMatchVbfLocationFiles(string mixNumber, List<FileMetaData> fileNames, SIIGOrderType orderType);

}