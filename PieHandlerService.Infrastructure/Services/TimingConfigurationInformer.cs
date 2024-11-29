using PieHandlerService.Infrastructure.Interfaces;


namespace PieHandlerService.Infrastructure.Services;

internal sealed class TimingConfigurationInformer : ITimingConfigurationInformer
{
    public required CircuitBreakerTimingConfiguration CircuitBreakerTimingConfiguration { get; set; }
}