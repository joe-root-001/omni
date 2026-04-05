using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Models;

public sealed record ImpactAnalysisResult(
    string RootEntityId,
    IReadOnlyCollection<KnowledgeEntity> AffectedEntities,
    IReadOnlyCollection<KnowledgeRelationship> TraversedRelationships,
    string Summary);
