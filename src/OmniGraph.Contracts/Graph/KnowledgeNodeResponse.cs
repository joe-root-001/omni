namespace OmniGraph.Contracts.Graph;

public sealed record KnowledgeNodeResponse(
    string Id,
    string Type,
    string DisplayName,
    string ExternalId,
    string RiskLevel,
    DateTimeOffset ValidFromUtc,
    DateTimeOffset? ValidToUtc,
    long Revision,
    IReadOnlyCollection<PropertyEntryResponse> Properties);
