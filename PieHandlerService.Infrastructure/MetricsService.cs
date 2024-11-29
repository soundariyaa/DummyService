using Microsoft.Extensions.Logging;
using PieHandlerService.Core.Interfaces;
using Prometheus;


namespace PieHandlerService.Infrastructure;

internal class MetricsService(ILogger<MetricsService> logger) : IMetricsService
{
    private readonly ILogger<MetricsService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private IReadOnlyDictionary<string, Counter> PieRequestCounters { get; } = new Dictionary<string, Counter>
    {
        {
            Constants.Metrics.HandledPieVehicleObjectRequestsMetric, Metrics.CreateCounter(Constants.Metrics.HandledPieVehicleObjectRequestsMetric,
                "Number of handled VehicleObject requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledPiePreFlashRequestsMetric, Metrics.CreateCounter(Constants.Metrics.HandledPiePreFlashRequestsMetric,
                "Number of handled PreFlash requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledPieEndOfLineRequestsMetric, Metrics.CreateCounter(Constants.Metrics.HandledPieEndOfLineRequestsMetric,
                "Number of handled EndOfline requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledPieErrorCode, Metrics.CreateCounter(Constants.Metrics.HandledPieErrorCode,
                "Number of handled Pie Error Codes by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledPieMissingVbfs, Metrics.CreateCounter(Constants.Metrics.HandledPieMissingVbfs,
                "Number of handled Pie Missing Vbfs by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        }
    };

    private IReadOnlyDictionary<string, Counter> StorageBroadcastCounters { get; } = new Dictionary<string, Counter>
    {
        {
            Constants.Metrics.HandledStorageEndOfLineBroadcastMetric, Metrics.CreateCounter(Constants.Metrics.HandledStorageEndOfLineBroadcastMetric,
                "Number of handled VehicleObject requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledStoragePreFlashBroadcastMetric, Metrics.CreateCounter(Constants.Metrics.HandledStoragePreFlashBroadcastMetric,
                "Number of handled PreFlash requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledStorageVehicleObjectBroadcastMetric, Metrics.CreateCounter(Constants.Metrics.HandledStorageVehicleObjectBroadcastMetric,
                "Number of handled EndOfline requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        }
    };

    private IReadOnlyDictionary<string, Counter> StorageSiigOrderCounters { get; } = new Dictionary<string, Counter>
    {
        {
            Constants.Metrics.HandledStorageVehicleObjectOrderMetric, Metrics.CreateCounter(Constants.Metrics.HandledStorageVehicleObjectOrderMetric,
                "Number of handled VehicleObject Order Storage by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledStoragePreFlashOrderMetric, Metrics.CreateCounter(Constants.Metrics.HandledStoragePreFlashOrderMetric,
                "Number of handled PreFlash Order Storage by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledStorageEndOfLineOrderMetric, Metrics.CreateCounter(Constants.Metrics.HandledStorageEndOfLineOrderMetric,
                "Number of handled EndOfline Order Storage by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        }
    };

    private IReadOnlyDictionary<string, Counter> MessageQueueCounters { get; } = new Dictionary<string, Counter>
    {
        {
            Constants.Metrics.HandledInboundQueueMetric, Metrics.CreateCounter(Constants.Metrics.HandledInboundQueueMetric,
                "Number of handled Inbound queues by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledOutboundQueueMetric, Metrics.CreateCounter(Constants.Metrics.HandledOutboundQueueMetric,
                "Number of handled Outbound queues by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        }
    };


    private IReadOnlyDictionary<string, Counter> DatabaseCounters { get; } = new Dictionary<string, Counter>
    {
        {
            Constants.Metrics.HandledSiigOrderDataMetric, Metrics.CreateCounter(Constants.Metrics.HandledSiigOrderDataMetric,
                "Number of handled SiigOrder database requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledPieResponseMessageDataMetric, Metrics.CreateCounter(Constants.Metrics.HandledPieResponseMessageDataMetric,
                "Number of handled PieResponseMessage requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        },
        {
            Constants.Metrics.HandledBroadcastContextMessageDataMetric, Metrics.CreateCounter(Constants.Metrics.HandledBroadcastContextMessageDataMetric,
                "Number of handled BroadcastContextMessage requests by result",
                new CounterConfiguration { LabelNames = new[] { Constants.Metrics.ResultKey } })
        }
    };


    public void IncreasePieRequestCounter(string counterName, params string[] labelParams)
    {
        if (!PieRequestCounters.ContainsKey(counterName))
        {
            LogMissingCounterError(counterName);
            return;
        }

        var counter = PieRequestCounters[counterName];

        counter.WithLabels(labelParams).Inc();
    }

    public void IncreaseStorageBroadcastCounters(string counterName, params string[] labelParams)
    {
        if (!StorageBroadcastCounters.ContainsKey(counterName))
        {
            LogMissingCounterError(counterName);
            return;
        }

        var counter = StorageBroadcastCounters[counterName];

        counter.WithLabels(labelParams).Inc();
    }

    public void IncreaseStorageSiigOrderCounters(string counterName, params string[] labelParams)
    {
        if (!StorageSiigOrderCounters.ContainsKey(counterName))
        {
            LogMissingCounterError(counterName);
            return;
        }

        var counter = StorageSiigOrderCounters[counterName];

        counter.WithLabels(labelParams).Inc();
    }

    public void IncreaseMessageCounters(string counterName, params string[] labelParams)
    {
        if (!MessageQueueCounters.ContainsKey(counterName))
        {
            LogMissingCounterError(counterName);
            return;
        }

        var counter = MessageQueueCounters[counterName];

        counter.WithLabels(labelParams).Inc();
    }

    public void IncreaseDatabaseCounters(string counterName, params string[] labelParams)
    {
        if (!DatabaseCounters.ContainsKey(counterName))
        {
            LogMissingCounterError(counterName);
            return;
        }

        var counter = DatabaseCounters[counterName];

        counter.WithLabels(labelParams).Inc();
    }

    private void LogMissingCounterError(string counterName)
    {
        _logger.LogError("Tried to access a counter with a name that does not exist: {CounterName}", counterName);
    }
}