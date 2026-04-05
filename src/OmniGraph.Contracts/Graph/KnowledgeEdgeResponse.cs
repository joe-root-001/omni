namespace OmniGraph.Contracts.Graph;

public sealed record KnowledgeEdgeResponse(
    string Id,
    string FromEntityId,
    string ToEntityId,
    string Type,
    double Confidence,
    string Explanation,
    DateTimeOffset ValidFromUtc,
    DateTimeOffset? ValidToUtc,
    long Revision);
