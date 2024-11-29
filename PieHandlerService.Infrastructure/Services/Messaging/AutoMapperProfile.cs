using AutoMapper;
using OE.MQ.IBM.XMS;

namespace PieHandlerService.Infrastructure.Services.Messaging;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<MqConfiguration, MqConnectionProperties?>()
            .ConvertUsing((mqConfiguration, _) =>
            {
                if (mqConfiguration == null)
                {
                    return null;
                }
                return new MqConnectionProperties
                (
                    mqConfiguration.Host!,
                    mqConfiguration.Port!.Value,
                    mqConfiguration.Channel!,
                    mqConfiguration.QueueManager!,
                    mqConfiguration.ConnectionMode,
                    mqConfiguration.ClientAcknowledge,
                    mqConfiguration.ClientId,
                    GetMqCredentials(mqConfiguration),
                    GetMqCertificateProperties(mqConfiguration),
                    GetConnectionInstance(mqConfiguration)!,
                    GetThrottleProperties(mqConfiguration)
                );
            });
    }

    private static MqCertificateProperties? GetMqCertificateProperties(MqConfiguration src) =>
        string.IsNullOrWhiteSpace(src.SslCipherSpec) || string.IsNullOrWhiteSpace(src.SslCertStore) || string.IsNullOrWhiteSpace(src.SslPeerName)
            ? null : new MqCertificateProperties(src.SslCipherSpec, src.SslCertLabel, src.SslCertStore, src.SslCertStorePassword, src.SslPeerName);

    private static MqCredentials? GetMqCredentials(MqConfiguration src) =>
        !string.IsNullOrWhiteSpace(src.UserName) && !string.IsNullOrWhiteSpace(src.Password) ? new MqCredentials(src.UserName, src.Password) : null;

    private static ThrottleProperties GetThrottleProperties(MqConfiguration src) 
        => new(src.MessageReceiveTimeoutMs, src.MessageIntervalTimeoutMs, src.SkipIntervalTimeoutOnMessageReceived ?? false);

    private static MqCustomReconnectProperties? GetConnectionInstance(MqConfiguration src)
    {
        if (src.TryReconnect != true)
        {
            return null;
        }
        return new MqCustomReconnectProperties
        {
            EnableCustomReconnection = true,
            DelayBetweenRetriesMs = src.DelayBetweenReconnectsMs ?? MqCustomReconnectProperties.DefaultDelayBetweenRetriesMs
        };
    }
}