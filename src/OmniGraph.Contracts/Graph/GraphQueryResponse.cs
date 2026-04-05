namespace OmniGraph.Contracts.Graph;

public sealed record GraphQueryResponse(
    string? PlannedQuery,
    IReadOnlyCollection<KnowledgeNodeResponse> Nodes,
    IReadOnlyCollection<KnowledgeEdgeResponse> Relationships);
