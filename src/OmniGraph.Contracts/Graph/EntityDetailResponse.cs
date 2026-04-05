namespace OmniGraph.Contracts.Graph;

public sealed record EntityDetailResponse(
    KnowledgeNodeResponse Entity,
    IReadOnlyCollection<KnowledgeEdgeResponse> Relationships);
