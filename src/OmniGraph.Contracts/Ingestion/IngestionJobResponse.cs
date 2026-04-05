namespace OmniGraph.Contracts.Ingestion;

public sealed record IngestionJobResponse(
    string JobId,
    string CorrelationId,
    string Status,
    DateTimeOffset SubmittedAtUtc,
    string ArtifactKind,
    string ArtifactUri);
