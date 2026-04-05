using OmniGraph.Domain.Models;

namespace OmniGraph.Application.Models;

public sealed record ExplainabilityResult(
    string RelationshipId,
    string Explanation,
    SourceArtifact SourceArtifact,
    IReadOnlyCollection<SourceEvidence> Evidence);
