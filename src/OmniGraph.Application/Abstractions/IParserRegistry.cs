using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Abstractions;

public interface IParserRegistry
{
    Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken);
}
