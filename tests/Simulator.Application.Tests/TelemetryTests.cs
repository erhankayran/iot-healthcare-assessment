using Moq;
using Simulator.Application.Abstractions;
using Simulator.Application.Services;
using Simulator.Domain.Constants;
using Simulator.Domain.Models;

namespace Simulator.Application.Tests;

public class RandomTelemetryGeneratorTests
{
    [Fact]
    public void Generate_ReturnsValuesWithinConfiguredRandomRanges()
    {
        var generator = new RandomTelemetryGenerator();

        for (var i = 0; i < 100; i++)
        {
            var telemetry = generator.Generate();

            Assert.InRange(telemetry.HeartRate, VitalSignRanges.HeartRateRandomMin, VitalSignRanges.HeartRateRandomMax);
            Assert.InRange(telemetry.SpO2, VitalSignRanges.SpO2RandomMin, VitalSignRanges.SpO2RandomMax);
        }
    }
}

public class CsvTelemetryParserTests
{
    private readonly CsvTelemetryParser _parser = new();

    [Fact]
    public void Parse_ReadsRowsWithHeader()
    {
        const string csv = """
            HeartRate,SpO2
            72,98
            68,97
            """;

        var readings = _parser.Parse(csv);

        Assert.Equal(2, readings.Count);
        Assert.Equal(72, readings[0].HeartRate);
        Assert.Equal(98, readings[0].SpO2);
    }

    [Fact]
    public void Parse_ThrowsForInvalidRow()
    {
        const string csv = """
            HeartRate,SpO2
            invalid,98
            """;

        var exception = Assert.Throws<InvalidOperationException>(() => _parser.Parse(csv));
        Assert.Contains("Invalid heart rate", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_ThrowsWhenCsvIsEmpty()
    {
        Assert.Throws<InvalidOperationException>(() => _parser.Parse("   "));
    }
}

public class TelemetrySimulationServiceTests
{
    [Fact]
    public async Task PublishManualAsync_PublishesValidTelemetry()
    {
        var publisherMock = new Mock<ITelemetryPublisher>();
        VitalTelemetry? published = null;

        publisherMock
            .Setup(publisher => publisher.PublishAsync(It.IsAny<VitalTelemetry>(), It.IsAny<CancellationToken>()))
            .Callback<VitalTelemetry, CancellationToken>((telemetry, _) => published = telemetry)
            .Returns(Task.CompletedTask);

        var service = new TelemetrySimulationService(publisherMock.Object);

        await service.PublishManualAsync(80, 97);

        Assert.NotNull(published);
        Assert.Equal(80, published!.HeartRate);
        Assert.Equal(97, published.SpO2);
        publisherMock.Verify(
            publisher => publisher.PublishAsync(It.IsAny<VitalTelemetry>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task PublishManualAsync_ThrowsForInvalidValues()
    {
        var publisherMock = new Mock<ITelemetryPublisher>();
        var service = new TelemetrySimulationService(publisherMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.PublishManualAsync(10, 97));
        publisherMock.Verify(
            publisher => publisher.PublishAsync(It.IsAny<VitalTelemetry>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
