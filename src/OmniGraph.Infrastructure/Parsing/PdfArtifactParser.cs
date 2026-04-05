using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Parsing;

public sealed class PdfArtifactParser : IArtifactParser
{
    public ArtifactKind Kind => ArtifactKind.Pdf;

    public Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var version = new VersionStamp(artifact.CapturedAtUtc, artifact.CapturedAtUtc, null, 1);
        var policy = new KnowledgeEntity(
            $"policy-{artifact.Id}",
            KnowledgeEntityType.Policy,
            artifact.Metadata.GetValueOrDefault("title", "Policy Document"),
            artifact.Uri,
            RiskLevel.Medium,
            version,
            artifact.Id,
            new Dictionary<string, string>
            {
                ["documentType"] = "policy",
                ["owner"] = artifact.Metadata.GetValueOrDefault("owner", "compliance")
            });

        return Task.FromResult(new ExtractionResult(artifact, new[] { policy }, Array.Empty<KnowledgeRelationship>(), 0.91, Array.Empty<string>()));
    }
}
