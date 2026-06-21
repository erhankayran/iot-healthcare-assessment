using Simulator.Domain.Models;

namespace Simulator.Application.Abstractions;

public interface ICsvTelemetryParser
{
    IReadOnlyList<VitalTelemetry> Parse(string csvContent);
}
