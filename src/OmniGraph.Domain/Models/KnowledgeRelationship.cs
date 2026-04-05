using OmniGraph.Domain.Enums;

namespace OmniGraph.Domain.Models;

public sealed record KnowledgeRelationship(
    string Id,
    string FromEntityId,
    string ToEntityId,
    KnowledgeRelationshipType Type,
    double Confidence,
    string Explanation,
    VersionStamp Version,
    string SourceArtifactId,
    List<SourceEvidence> Evidence);
