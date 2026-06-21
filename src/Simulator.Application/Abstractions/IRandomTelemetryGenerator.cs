using Simulator.Domain.Models;

namespace Simulator.Application.Abstractions;

public interface IRandomTelemetryGenerator
{
    VitalTelemetry Generate(DateTimeOffset? timestamp = null);
}
