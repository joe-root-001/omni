using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Abstractions;

public interface IArtifactParser
{
    ArtifactKind Kind { get; }

    Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken);
}
