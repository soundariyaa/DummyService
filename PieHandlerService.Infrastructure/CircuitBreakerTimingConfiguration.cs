namespace PieHandlerService.Infrastructure;

public class CircuitBreakerTimingConfiguration
{
    public double OpenBrokerTimeoutInSeconds { get; set; }
    public int RetriesBeforeOpening { get; set; }
}