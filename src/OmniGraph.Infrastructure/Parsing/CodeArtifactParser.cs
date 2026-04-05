using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Parsing;

public sealed class CodeArtifactParser : IArtifactParser
{
    public ArtifactKind Kind => ArtifactKind.Code;

    public Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var version = new VersionStamp(artifact.CapturedAtUtc, artifact.CapturedAtUtc, null, 1);
        var function = new KnowledgeEntity(
            $"function-{artifact.Id}",
            KnowledgeEntityType.Function,
            artifact.Metadata.GetValueOrDefault("entryPoint", "ParsedFunction"),
            artifact.Uri,
            RiskLevel.Low,
            version,
            artifact.Id,
            new Dictionary<string, string>
            {
                ["language"] = artifact.Metadata.GetValueOrDefault("language", "unknown"),
                ["analysis"] = "ast"
            });

        return Task.FromResult(new ExtractionResult(artifact, new[] { function }, Array.Empty<KnowledgeRelationship>(), 0.96, Array.Empty<string>()));
    }
}
