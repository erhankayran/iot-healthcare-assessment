using Microsoft.Extensions.DependencyInjection;
using Simulator.Application.Abstractions;
using Simulator.Application.Services;

namespace Simulator.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IRandomTelemetryGenerator, RandomTelemetryGenerator>();
        services.AddSingleton<ICsvTelemetryParser, CsvTelemetryParser>();
        services.AddScoped<TelemetrySimulationService>();

        return services;
    }
}
