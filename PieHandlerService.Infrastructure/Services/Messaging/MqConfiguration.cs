namespace PieHandlerService.Infrastructure.Services.Messaging;

public sealed class MqConfiguration
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? QueueManager { get; set; }
    public string? Channel { get; set; }
    public string? QueueName { get; set; }
    public int? ConnectionMode { get; set; }
    public bool? ClientAcknowledge { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? SslCipherSpec { get; set; }
    public string SslCertLabel { get; set; }
    public string? SslCertStore { get; set; }
    public string? SslCertStorePassword { get; set; }
    public string? SslPeerName { get; set; }
    public string? ClientId { get; set; }
    public bool? TryReconnect { get; set; }
    public int? DelayBetweenReconnectsMs { get; set; }
    public int? MessageReceiveTimeoutMs { get; set; }
    public int? MessageIntervalTimeoutMs { get; set; }
    public bool? SkipIntervalTimeoutOnMessageReceived { get; set; }

    public bool DisableMq { get; set; }

    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Host) &&
        Port.HasValue &&
        !string.IsNullOrWhiteSpace(QueueManager) &&
        !string.IsNullOrWhiteSpace(Channel) && !string.IsNullOrWhiteSpace(QueueName);

    public override string ToString() =>
        $"Host: {Host}, Port: {Port}, QueueManager: {QueueManager}, Channel: {Channel}, QueueName: {QueueName}, " +
        $"ConnectionMode: {ConnectionMode}, ClientAcknowledge: {ClientAcknowledge}, UserId: {UserName}, " +
        $"Password: {(!string.IsNullOrWhiteSpace(Password) ? "****" : string.Empty)}, " +
        $"SslCipherSpec: {SslCipherSpec}, SslCertLabel: {SslCertLabel}, " +
        $"SslCertStorePassword: {(!string.IsNullOrWhiteSpace(SslCertStorePassword) ? "****" : string.Empty)}, " +
        $"SslPeerName: {SslPeerName}, ClientId: {ClientId}, TryReconnect: {TryReconnect}, " +
        $"MessageReceiveTimeoutMs: {MessageReceiveTimeoutMs}, MessageIntervalTimeoutMs: {MessageIntervalTimeoutMs}, " +
        $"SkipIntervalTimeoutOnMessageReceived: {SkipIntervalTimeoutOnMessageReceived}";
}