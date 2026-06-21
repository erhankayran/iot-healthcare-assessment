using Simulator.Application.Abstractions;
using Simulator.Domain.Constants;
using Simulator.Domain.Models;

namespace Simulator.Application.Services;

public sealed class RandomTelemetryGenerator : IRandomTelemetryGenerator
{
    private readonly Random _random = new();

    public VitalTelemetry Generate(DateTimeOffset? timestamp = null)
    {
        var heartRate = _random.Next(
            VitalSignRanges.HeartRateRandomMin,
            VitalSignRanges.HeartRateRandomMax + 1);

        var spO2 = _random.Next(
            VitalSignRanges.SpO2RandomMin,
            VitalSignRanges.SpO2RandomMax + 1);

        return new VitalTelemetry(heartRate, spO2, timestamp ?? DateTimeOffset.UtcNow);
    }
}
