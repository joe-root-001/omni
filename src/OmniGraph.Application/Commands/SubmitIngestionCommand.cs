using OmniGraph.Domain.Enums;

namespace OmniGraph.Application.Commands;

public sealed record SubmitIngestionCommand(
    string ArtifactUri,
    ArtifactKind ArtifactKind,
    string RequestedBy,
    string? CorrelationId,
    Dictionary<string, string>? Metadata);
