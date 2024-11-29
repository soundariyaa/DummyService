namespace PieHandlerService.Core.Models;

public sealed class EraseData
{
    public ulong? EraseStartAddress { get; set; }
    public ulong? EraseLength { get; set; }
}