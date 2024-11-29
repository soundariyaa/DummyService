namespace PieHandlerService.Core.Interfaces;

public interface IMetricsService
{
    void IncreasePieRequestCounter(string counterName, params string[] labelParams);

    void IncreaseDatabaseCounters(string counterName, params string[] labelParams);

    void IncreaseMessageCounters(string counterName, params string[] labelParams);

    void IncreaseStorageSiigOrderCounters(string counterName, params string[] labelParams);

    void IncreaseStorageBroadcastCounters(string counterName, params string[] labelParams);
}