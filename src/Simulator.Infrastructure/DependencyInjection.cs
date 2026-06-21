using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Simulator.Application.Abstractions;
using Simulator.Infrastructure.Mqtt;
using Simulator.Infrastructure.Options;

namespace Simulator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ThingsBoardOptions>(
            configuration.GetSection(ThingsBoardOptions.SectionName));

        services.AddSingleton<ITelemetryPublisher, MqttTelemetryPublisher>();

        return services;
    }
}
