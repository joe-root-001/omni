using OmniGraph.Domain.Enums;

namespace OmniGraph.Domain.Models;

public sealed record IngestionJob(
    string Id,
    string CorrelationId,
    string RequestedBy,
    ArtifactKind ArtifactKind,
    string ArtifactUri,
    DateTimeOffset SubmittedAtUtc,
    IngestionStatus Status,
    Dictionary<string, string> Metadata);
