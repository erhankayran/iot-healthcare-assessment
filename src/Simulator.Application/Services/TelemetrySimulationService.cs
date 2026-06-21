using Simulator.Application.Abstractions;
using Simulator.Domain.Models;
using Simulator.Domain.Validation;

namespace Simulator.Application.Services;

public sealed class TelemetrySimulationService
{
    private readonly ITelemetryPublisher _publisher;

    public TelemetrySimulationService(ITelemetryPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishManualAsync(
        int heartRate,
        int spO2,
        CancellationToken cancellationToken = default)
    {
        var telemetry = new VitalTelemetry(heartRate, spO2, DateTimeOffset.UtcNow);

        if (!VitalTelemetryValidator.TryValidate(telemetry, out var errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }

        await _publisher.PublishAsync(telemetry, cancellationToken);
    }

    public async Task PublishAsync(
        VitalTelemetry telemetry,
        CancellationToken cancellationToken = default)
    {
        if (!VitalTelemetryValidator.TryValidate(telemetry, out var errorMessage))
        {
            throw new InvalidOperationException(errorMessage);
        }

        await _publisher.PublishAsync(telemetry, cancellationToken);
    }
}
