using System.Threading.Channels;


namespace PieHandlerService.Infrastructure.Services.Helper;

public class ChannelConfig
{
    internal static BoundedChannelFullMode DefaultFullMode => BoundedChannelFullMode.Wait;
    internal static int DefaultCapacity => 1024;
    public static int DefaultPriorityChannelPrecedenceCount => 4;
    public static int DefaultPriorityChannelCapacity => DefaultCapacity / 5;
    internal static int DefaultChannelReadDelayMs => 10;
    internal static int DefaultPeriodIntervalSeconds => 10;
    public BoundedChannelFullMode FullMode { get; set; } = DefaultFullMode;
    public int Capacity { get; set; } = DefaultCapacity;
    public int ChannelReadDelayMs { get; set; } = DefaultChannelReadDelayMs;
    public int PeriodicCheckIntervalSeconds { get; set; } = DefaultPeriodIntervalSeconds;
    public int? PriorityChannelPrecedenceCount { get; set; } = DefaultPriorityChannelPrecedenceCount;
}