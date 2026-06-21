namespace Simulator.Domain.Models;

public sealed record VitalTelemetry(
    int HeartRate,
    int SpO2,
    DateTimeOffset Timestamp);
