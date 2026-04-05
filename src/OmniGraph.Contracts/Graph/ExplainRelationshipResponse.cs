namespace OmniGraph.Contracts.Graph;

public sealed record ExplainRelationshipResponse(
    string RelationshipId,
    string Explanation,
    string SourceArtifactId,
    string SourceArtifactUri,
    IReadOnlyCollection<SourceEvidenceResponse> Evidence);
