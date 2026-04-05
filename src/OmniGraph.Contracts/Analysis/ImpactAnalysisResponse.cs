using OmniGraph.Contracts.Graph;

namespace OmniGraph.Contracts.Analysis;

public sealed record ImpactAnalysisResponse(
    string RootEntityId,
    string Summary,
    IReadOnlyCollection<KnowledgeNodeResponse> AffectedEntities,
    IReadOnlyCollection<KnowledgeEdgeResponse> TraversedRelationships);
