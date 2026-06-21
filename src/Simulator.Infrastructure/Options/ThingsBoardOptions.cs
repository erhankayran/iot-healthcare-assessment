namespace Simulator.Infrastructure.Options;

public sealed class ThingsBoardOptions
{
    public const string SectionName = "ThingsBoard";

    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 1883;

    public string AccessToken { get; set; } = string.Empty;

    public string TelemetryTopic { get; set; } = "v1/devices/me/telemetry";

    public int PublishIntervalMilliseconds { get; set; } = 2000;
}
