using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OmniGraph.Application.Abstractions;
using OmniGraph.Infrastructure.Explainability;
using OmniGraph.Infrastructure.Messaging;
using OmniGraph.Infrastructure.Options;
using OmniGraph.Infrastructure.Persistence;
using OmniGraph.Infrastructure.Parsing;
using OmniGraph.Infrastructure.Querying;

namespace OmniGraph.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOmniGraphInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.SectionName));
        services.Configure<PostgreSqlOptions>(configuration.GetSection(PostgreSqlOptions.SectionName));
        services.Configure<Neo4jOptions>(configuration.GetSection(Neo4jOptions.SectionName));
        services.Configure<ObjectStorageOptions>(configuration.GetSection(ObjectStorageOptions.SectionName));
        services.Configure<AIOptions>(configuration.GetSection(AIOptions.SectionName));

        services.AddSingleton<InMemoryKnowledgeStore>();
        services.AddSingleton<IGraphRepository, InMemoryGraphRepository>();
        services.AddSingleton<IExplainabilityService, InMemoryExplainabilityService>();
        services.AddSingleton<INaturalLanguageQueryTranslator, DeterministicNaturalLanguageQueryTranslator>();
        services.AddSingleton<IIngestionPublisher, KafkaIngestionPublisher>();
        services.AddSingleton<IParserRegistry, ParserRegistry>();
        services.AddSingleton<IArtifactParser, ExcelArtifactParser>();
        services.AddSingleton<IArtifactParser, PdfArtifactParser>();
        services.AddSingleton<IArtifactParser, ImageArtifactParser>();
        services.AddSingleton<IArtifactParser, LogArtifactParser>();
        services.AddSingleton<IArtifactParser, CodeArtifactParser>();

        return services;
    }
}
