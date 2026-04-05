using OmniGraph.Domain.Enums;

namespace OmniGraph.Application.Models;

public sealed record GraphQueryRequest(
    KnowledgeEntityType? EntityType,
    RiskLevel? MinimumRisk,
    string? Text,
    string? RelatedToPolicy,
    int Depth,
    int Limit);
