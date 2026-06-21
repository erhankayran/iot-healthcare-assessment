using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using Simulator.Application.Abstractions;
using Simulator.Domain.Models;
using Simulator.Infrastructure.Options;

namespace Simulator.Infrastructure.Mqtt;

public sealed class MqttTelemetryPublisher : ITelemetryPublisher, IAsyncDisposable
{
    private readonly ThingsBoardOptions _options;
    private readonly ILogger<MqttTelemetryPublisher> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private bool _isConnected;

    public MqttTelemetryPublisher(
        IOptions<ThingsBoardOptions> options,
        ILogger<MqttTelemetryPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;
        _mqttClient = new MqttClientFactory().CreateMqttClient();
    }

    public async Task PublishAsync(VitalTelemetry telemetry, CancellationToken cancellationToken = default)
    {
        await EnsureConnectedAsync(cancellationToken);

        var payload = JsonSerializer.Serialize(new
        {
            heartRate = telemetry.HeartRate,
            spo2 = telemetry.SpO2,
            ts = telemetry.Timestamp.ToUnixTimeMilliseconds()
        });

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(_options.TelemetryTopic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        var result = await _mqttClient.PublishAsync(message, cancellationToken);
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                $"MQTT publish failed with reason: {result.ReasonCode}.");
        }

        _logger.LogInformation(
            "Published telemetry HR={HeartRate}, SpO2={SpO2}",
            telemetry.HeartRate,
            telemetry.SpO2);
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient.IsConnected)
        {
            return;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_mqttClient.IsConnected)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.AccessToken))
            {
                throw new InvalidOperationException(
                    "ThingsBoard access token is not configured. Update appsettings.json.");
            }

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_options.Host, _options.Port)
                .WithCredentials(_options.AccessToken, string.Empty)
                .WithClientId($"healthcare-simulator-{Guid.NewGuid():N}")
                .WithCleanSession()
                .Build();

            var connectResult = await _mqttClient.ConnectAsync(options, cancellationToken);
            if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
            {
                throw new InvalidOperationException(
                    $"Unable to connect to ThingsBoard MQTT broker. Result: {connectResult.ResultCode}");
            }

            _isConnected = true;
            _logger.LogInformation(
                "Connected to ThingsBoard MQTT broker at {Host}:{Port}",
                _options.Host,
                _options.Port);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_isConnected && _mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync();
        }

        _mqttClient.Dispose();
        _connectionLock.Dispose();
    }
}
