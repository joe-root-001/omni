namespace OmniGraph.Contracts.Graph;

public sealed record SourceEvidenceResponse(
    string ArtifactId,
    string Section,
    string Snippet,
    string ExtractionRule);
