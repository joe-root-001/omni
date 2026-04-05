using OmniGraph.Domain.Enums;

namespace OmniGraph.Contracts.Ingestion;

public sealed record SubmitIngestionRequest(
    string ArtifactUri,
    ArtifactKind ArtifactKind,
    string RequestedBy,
    string? CorrelationId,
    Dictionary<string, string>? Metadata);
