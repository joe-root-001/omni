namespace OmniGraph.Domain.Models;

public sealed record ExtractionResult(
    SourceArtifact Artifact,
    IReadOnlyCollection<KnowledgeEntity> Entities,
    IReadOnlyCollection<KnowledgeRelationship> Relationships,
    double QualityScore,
    IReadOnlyCollection<string> Warnings);
