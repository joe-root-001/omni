using Microsoft.Extensions.DependencyInjection;
using OmniGraph.Application.Services;

namespace OmniGraph.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOmniGraphApplication(this IServiceCollection services)
    {
        services.AddScoped<IngestionOrchestrator>();
        services.AddScoped<QueryService>();
        services.AddScoped<GraphExplorationService>();

        return services;
    }
}
