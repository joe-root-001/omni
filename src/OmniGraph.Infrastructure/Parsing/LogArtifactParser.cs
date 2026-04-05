using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Parsing;

public sealed class LogArtifactParser : IArtifactParser
{
    public ArtifactKind Kind => ArtifactKind.Log;

    public Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var version = new VersionStamp(artifact.CapturedAtUtc, artifact.CapturedAtUtc, null, 1);
        var evt = new KnowledgeEntity(
            $"event-{artifact.Id}",
            KnowledgeEntityType.Event,
            "Parsed Log Event",
            artifact.Uri,
            RiskLevel.Medium,
            version,
            artifact.Id,
            new Dictionary<string, string>
            {
                ["service"] = artifact.Metadata.GetValueOrDefault("service", "unknown"),
                ["parser"] = "event-parser"
            });

        return Task.FromResult(new ExtractionResult(artifact, new[] { evt }, Array.Empty<KnowledgeRelationship>(), 0.93, Array.Empty<string>()));
    }
}
