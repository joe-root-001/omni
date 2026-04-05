using OmniGraph.Domain.Enums;

namespace OmniGraph.Domain.Models;

public sealed record KnowledgeEntity(
    string Id,
    KnowledgeEntityType Type,
    string DisplayName,
    string ExternalId,
    RiskLevel RiskLevel,
    VersionStamp Version,
    string SourceArtifactId,
    Dictionary<string, string> Properties);
