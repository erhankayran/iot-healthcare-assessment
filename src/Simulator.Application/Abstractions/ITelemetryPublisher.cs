using Simulator.Domain.Models;

namespace Simulator.Application.Abstractions;

public interface ITelemetryPublisher
{
    Task PublishAsync(VitalTelemetry telemetry, CancellationToken cancellationToken = default);
}
