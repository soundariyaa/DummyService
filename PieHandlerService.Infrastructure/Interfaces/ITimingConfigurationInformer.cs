namespace PieHandlerService.Infrastructure.Interfaces;

internal interface ITimingConfigurationInformer
{
    CircuitBreakerTimingConfiguration CircuitBreakerTimingConfiguration { get; set; }
}