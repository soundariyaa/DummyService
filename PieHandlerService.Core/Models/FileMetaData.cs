namespace PieHandlerService.Core.Models;

public class FileMetaData ( string FileName, uint DataFileChecksum)
{
    public string FileName { get; set; } = FileName;

    public uint DataFileCheckSum { get; set; } = DataFileChecksum;
}