using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Parsing;

public sealed class ImageArtifactParser : IArtifactParser
{
    public ArtifactKind Kind => ArtifactKind.Image;

    public Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var version = new VersionStamp(artifact.CapturedAtUtc, artifact.CapturedAtUtc, null, 1);
        var document = new KnowledgeEntity(
            $"document-{artifact.Id}",
            KnowledgeEntityType.Document,
            "OCR Extracted Image",
            artifact.Uri,
            RiskLevel.Low,
            version,
            artifact.Id,
            new Dictionary<string, string>
            {
                ["ocr"] = "enabled",
                ["caption"] = artifact.Metadata.GetValueOrDefault("caption", "Operational screenshot")
            });

        return Task.FromResult(new ExtractionResult(artifact, new[] { document }, Array.Empty<KnowledgeRelationship>(), 0.87, Array.Empty<string>()));
    }
}
