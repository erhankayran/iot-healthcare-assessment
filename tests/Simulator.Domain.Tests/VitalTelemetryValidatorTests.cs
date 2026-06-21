using Simulator.Domain.Constants;
using Simulator.Domain.Models;
using Simulator.Domain.Validation;

namespace Simulator.Domain.Tests;

public class VitalTelemetryValidatorTests
{
    [Theory]
    [InlineData(72, 98)]
    [InlineData(40, 85)]
    [InlineData(200, 100)]
    public void TryValidate_ReturnsTrue_ForValidValues(int heartRate, int spO2)
    {
        var telemetry = new VitalTelemetry(heartRate, spO2, DateTimeOffset.UtcNow);

        var isValid = VitalTelemetryValidator.TryValidate(telemetry, out var errorMessage);

        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    [Theory]
    [InlineData(39, 98)]
    [InlineData(201, 98)]
    public void TryValidate_ReturnsFalse_ForInvalidHeartRate(int heartRate, int spO2)
    {
        var telemetry = new VitalTelemetry(heartRate, spO2, DateTimeOffset.UtcNow);

        var isValid = VitalTelemetryValidator.TryValidate(telemetry, out var errorMessage);

        Assert.False(isValid);
        Assert.Contains("Heart rate", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(72, 84)]
    [InlineData(72, 101)]
    public void TryValidate_ReturnsFalse_ForInvalidSpO2(int heartRate, int spO2)
    {
        var telemetry = new VitalTelemetry(heartRate, spO2, DateTimeOffset.UtcNow);

        var isValid = VitalTelemetryValidator.TryValidate(telemetry, out var errorMessage);

        Assert.False(isValid);
        Assert.Contains("SpO2", errorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void VitalSignRanges_DefinesExpectedRandomBounds()
    {
        Assert.True(VitalSignRanges.HeartRateRandomMin >= VitalSignRanges.HeartRateMin);
        Assert.True(VitalSignRanges.HeartRateRandomMax <= VitalSignRanges.HeartRateMax);
        Assert.True(VitalSignRanges.SpO2RandomMin >= VitalSignRanges.SpO2Min);
        Assert.True(VitalSignRanges.SpO2RandomMax <= VitalSignRanges.SpO2Max);
    }
}
