using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Models;

public sealed record GraphQueryResult(
    IReadOnlyCollection<KnowledgeEntity> Nodes,
    IReadOnlyCollection<KnowledgeRelationship> Relationships,
    string? PlannedQuery);
