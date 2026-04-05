using OmniGraph.Application.Abstractions;
using OmniGraph.Domain.Enums;
using OmniGraph.Domain.Models;

namespace OmniGraph.Infrastructure.Parsing;

public sealed class ParserRegistry(IEnumerable<IArtifactParser> parsers) : IParserRegistry
{
    private readonly Dictionary<ArtifactKind, IArtifactParser> _parsers =
        parsers.ToDictionary(parser => parser.Kind, parser => parser);

    public Task<ExtractionResult> ParseAsync(SourceArtifact artifact, Stream content, CancellationToken cancellationToken)
    {
        if (_parsers.TryGetValue(artifact.Kind, out var parser))
        {
            return parser.ParseAsync(artifact, content, cancellationToken);
        }

        throw new NotSupportedException($"No parser registered for artifact kind '{artifact.Kind}'.");
    }
}
