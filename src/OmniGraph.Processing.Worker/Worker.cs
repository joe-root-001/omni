using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Processing.Worker;

public sealed class Worker(
    ILogger<Worker> logger,
    IParserRegistry parserRegistry,
    IGraphRepository graphRepository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var artifact = new SourceArtifact(
            "artifact-demo-image",
            ArtifactKind.Image,
            "file://incoming/ops-dashboard.png",
            "sha256-demo-image",
            DateTimeOffset.UtcNow,
            new Dictionary<string, string>
            {
                ["caption"] = "Payments operations dashboard"
            });

        var extraction = await parserRegistry.ParseAsync(artifact, Stream.Null, stoppingToken);
        await graphRepository.UpsertExtractionAsync(extraction, stoppingToken);

        WorkerLogMessages.ProcessingCompleted(
            logger,
            artifact.Kind,
            extraction.Entities.Count,
            extraction.Relationships.Count);

        while (!stoppingToken.IsCancellationRequested)
        {
            WorkerLogMessages.WorkerIdle(logger);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

internal static partial class WorkerLogMessages
{
    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Processed {ArtifactKind} artifact into {EntityCount} entities and {RelationshipCount} relationships")]
    public static partial void ProcessingCompleted(
        ILogger logger,
        ArtifactKind artifactKind,
        int entityCount,
        int relationshipCount);

    [LoggerMessage(
        EventId = 2002,
        Level = LogLevel.Information,
        Message = "Processing worker idle; parser registry and graph projection path are ready.")]
    public static partial void WorkerIdle(ILogger logger);
}
